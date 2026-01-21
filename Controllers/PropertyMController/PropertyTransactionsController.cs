using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIETE.Data;
using SIETE.Models;
using SIETE.Models.Property;

namespace SIETE.Controllers.PropertyMController
{
    public class PropertyTransactionsController : Controller
    {
        private readonly SIETEContext _context;

        public PropertyTransactionsController(SIETEContext context)
        {
            _context = context;
        }

        // GET: PropertyTransactions
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.PropertyTransaction
                .Include(pt => pt.PropertyTransactionDetails)
                .OrderByDescending(pt => pt.TransactionDate)
                .ToListAsync();
            return View(transactions);
        }

        // GET: PropertyTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var propertyTransaction = await _context.PropertyTransaction
                .Include(pt => pt.PropertyTransactionDetails)
                    .ThenInclude(ptd => ptd.Property)
                        .ThenInclude(p => p.PropertyAssignmentHistory)
                            .ThenInclude(h => h.Employee)
                                .ThenInclude(e => e.Office)
                .Include(pt => pt.PropertyTransactionDetails)
                    .ThenInclude(ptd => ptd.Property)
                        .ThenInclude(p => p.PropertyCard)
                .Include(pt => pt.PropertyTransactionDetails)
                .FirstOrDefaultAsync(m => m.TransactionID == id);

            if (propertyTransaction == null)
            {
                return NotFound();
            }

            return PartialView(propertyTransaction);
        }

        private async Task<PriceThreshold?> GetLatestThreshold()
        {
            return await _context.PriceThreshold
                .OrderByDescending(t => t.EffectiveDate)
                .FirstOrDefaultAsync();
        }

        [HttpGet]
        public IActionResult AcquireProperties()
        {
            var viewModel = new PropertyTransactionVM
            {
                SupplierList = new SelectList(_context.Supplier, "SupplierID", "SupplierName"),
                FundClusterList = new SelectList(_context.FundCluster, "FundClusterID", "FundClusterName"),
                // ✅ This line ensures index [0] is valid
                PropertyTransactionDetail =
                [
                    new PropertyTransactionDetail(){

                    }


                ]
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcquireProperties(PropertyTransactionVM propertyVM)
        {
            // Start a database transaction
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get the latest price threshold for categorization
                var threshold = await GetLatestThreshold();
                if (threshold == null)
                {
                    ModelState.AddModelError("", "No price threshold found. Please set up price thresholds first.");
                    return View(propertyVM);
                }

                // Calculate total amount for the transaction
                decimal totalAmount = propertyVM.PropertyTransactionDetail
                    .Where(d => d != null && d.Property != null)
                    .Sum(d => d.Quantity * d.UnitCost);

                // Create the transaction record
                var transaction = new PropertyTransaction
                {
                    Type = TransactionTypes.Aquired,
                    TransactionDate = propertyVM.Transaction.TransactionDate,
                    Remarks = propertyVM.Transaction.Remarks
                };
                _context.PropertyTransaction.Add(transaction);
                await _context.SaveChangesAsync(); // Save to get TransactionID

                var transactionDetails = new List<PropertyTransactionDetail>();
                var errors = new List<string>();

                // Process each property in the transaction
                foreach (var detail in propertyVM.PropertyTransactionDetail.Where(d => d != null && d.Property != null))
                {
                    var inputProperty = detail.Property;
                    var quantity = detail.Quantity;
                    var unitCost = detail.UnitCost;

                    // Enhanced validation
                    if (!ValidatePropertyInput(inputProperty, quantity, unitCost, out string validationError))
                    {
                        errors.Add(validationError);
                        continue;
                    }

                    try
                    {
                        // Find or create PropertyCard with case-insensitive comparison
                        var propertyCard = await _context.PropertyCard.FirstOrDefaultAsync(pc =>
                            pc.PropertyCardName.ToUpper().Trim() == inputProperty.PropertyName.ToUpper().Trim() &&
                            pc.PropertyDescription.ToUpper().Trim() == (inputProperty.Description ?? "").ToUpper().Trim() &&
                            pc.PropertyUnitMeasurement.ToUpper().Trim() == inputProperty.UnitMeasurement.ToUpper().Trim());

                        if (propertyCard == null)
                        {
                            propertyCard = new PropertyCard
                            {
                                PropertyCardName = inputProperty.PropertyName,
                                PropertyDescription = inputProperty.Description ?? "",
                                PropertyUnitMeasurement = inputProperty.UnitMeasurement,
                                CurrentStockQuantity = quantity,
                                TotalAmount = quantity * unitCost,
                                LastUpdated = DateTime.Now
                            };
                            _context.PropertyCard.Add(propertyCard);
                        }
                        else
                        {
                            propertyCard.CurrentStockQuantity += quantity;
                            propertyCard.TotalAmount += quantity * unitCost;
                            propertyCard.LastUpdated = DateTime.Now;
                            _context.PropertyCard.Update(propertyCard);
                        }
                        await _context.SaveChangesAsync();

                        // Create new Property with category determination
                        var newProperty = new Property
                        {
                            SupplierID = inputProperty.SupplierID,
                            FundClusterID = inputProperty.FundClusterID,
                            StockPropNo = inputProperty.StockPropNo,
                            PropertyName = inputProperty.PropertyName,
                            Description = inputProperty.Description,
                            UnitMeasurement = inputProperty.UnitMeasurement,
                            UnitCost = unitCost,
                            StockQuantity = quantity,
                            DateAcquired = DateTime.Now,
                            Status = "Active",
                            PropertyCardID = propertyCard.PropertyCardID
                        };

                        // Determine property category based on price threshold
                        newProperty.UpdateCategory(threshold);
                        _context.Property.Add(newProperty);
                        await _context.SaveChangesAsync();

                        // Create transaction detail
                        transactionDetails.Add(new PropertyTransactionDetail
                        {
                            TransactionID = transaction.TransactionID,
                            PropertyID = newProperty.PropertyID,
                            Quantity = quantity,
                            UnitCost = unitCost,
                            TotalAmount = quantity * unitCost
                        });
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error processing property {inputProperty.PropertyName}: {ex.Message}");
                    }
                }

                if (transactionDetails.Any())
                {
                    _context.PropertyTransactionDetail.AddRange(transactionDetails);
                    await _context.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                    TempData["SuccessMessage"] = "Properties acquired successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    await dbTransaction.RollbackAsync();
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    // Reload the lists for the view
                    propertyVM.SupplierList = new SelectList(_context.Supplier, "SupplierID", "SupplierName");
                    propertyVM.FundClusterList = new SelectList(_context.FundCluster, "FundClusterID", "FundClusterName");
                    return View(propertyVM);
                }
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                // Reload the lists for the view
                propertyVM.SupplierList = new SelectList(_context.Supplier, "SupplierID", "SupplierName");
                propertyVM.FundClusterList = new SelectList(_context.FundCluster, "FundClusterID", "FundClusterName");
                return View(propertyVM);
            }
        }

        // Helper method for property validation
        private bool ValidatePropertyInput(Property inputProperty, int quantity, decimal unitCost, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (quantity <= 0)
            {
                errorMessage = $"Invalid quantity for property: {inputProperty.PropertyName}";
                return false;
            }

            if (unitCost <= 0)
            {
                errorMessage = $"Invalid unit cost for property: {inputProperty.PropertyName}";
                return false;
            }

            if (string.IsNullOrWhiteSpace(inputProperty.PropertyName))
            {
                errorMessage = "Property name is required";
                return false;
            }

            if (string.IsNullOrWhiteSpace(inputProperty.UnitMeasurement))
            {
                errorMessage = "Unit of measurement is required";
                return false;
            }

            if (inputProperty.SupplierID <= 0)
            {
                errorMessage = "Supplier is required";
                return false;
            }

            if (inputProperty.FundClusterID <= 0)
            {
                errorMessage = "Fund cluster is required";
                return false;
            }

            if (string.IsNullOrWhiteSpace(inputProperty.StockPropNo))
            {
                errorMessage = "Stock/Property number is required";
                return false;
            }

            return true;
        }


        [HttpGet]
        public IActionResult TransferProperties()
        {
            var viewModel = new PropertyTransactionVM
            {
                EmployeeList = new SelectList(_context.Employee, "EmployeeID", "FullName"),
                PropertyList = new SelectList(_context.Property
                    .Where(p => p.StockQuantity > 0 && p.Status != "Out of Stock")
                    .Select(p => new {
                        p.PropertyID,
                        DisplayText = $"{p.PropertyName}|{p.Description}|{p.UnitMeasurement}|{p.UnitCost} | {p.StockQuantity}"
                    }), "PropertyID", "DisplayText"),
                PropertyTransactionDetail = new List<PropertyTransactionDetail>()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransferProperties(PropertyTransactionVM propertyVM)
        {
            // Debug logging
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            if (!ModelState.IsValid)
            {
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }
            }

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Log the incoming data
                Console.WriteLine($"EmployeeID: {propertyVM.EmployeeID}");
                Console.WriteLine($"PropertyTransactionDetail Count: {propertyVM.PropertyTransactionDetail?.Count ?? 0}");
                foreach (var detail in propertyVM.PropertyTransactionDetail ?? new List<PropertyTransactionDetail>())
                {
                    Console.WriteLine($"Detail - PropertyID: {detail.PropertyID}, Quantity: {detail.Quantity}");
                }

                var transaction = new PropertyTransaction
                {
                    Type = TransactionTypes.Transferred,
                    TransactionDate = DateTime.UtcNow,
                    Remarks = propertyVM.Transaction?.Remarks
                };
                _context.PropertyTransaction.Add(transaction);
                
                try
                {
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Transaction saved with ID: {transaction.TransactionID}");
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Error saving transaction: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }
                    throw;
                }

                foreach (var detail in propertyVM.PropertyTransactionDetail)
                {
                    if (detail == null) continue;

                    detail.EmployeeID = propertyVM.EmployeeID;
                    var property = await _context.Property
                        .Include(p => p.PropertyCard)
                        .FirstOrDefaultAsync(p => p.PropertyID == detail.PropertyID);

                    var employee = await _context.Employee
                        .Include(e => e.Office)
                        .FirstOrDefaultAsync(e => e.EmployeeID == detail.EmployeeID);

                    // Validate if there's enough quantity to transfer
                    if (property.StockQuantity < detail.Quantity)
                    {
                        await dbTransaction.RollbackAsync();
                        ModelState.AddModelError("", $"Not enough quantity available for property {property.PropertyName}. Available: {property.StockQuantity}, Requested: {detail.Quantity}");
                        return View(propertyVM);
                    }

                    // Update property status and quantity
                    var originalQuantity = property.StockQuantity;
                    property.StockQuantity = originalQuantity - detail.Quantity;
                    if (property.StockQuantity == 0)
                    {
                        property.Status = "Out of Stock";
                    }
                    _context.Property.Update(property);

                    var propertyCard = await _context.PropertyCard.FindAsync(property.PropertyCardID);
                    if (propertyCard != null)
                    {
                        var originalCardQuantity = propertyCard.CurrentStockQuantity;
                        propertyCard.CurrentStockQuantity = originalCardQuantity - detail.Quantity;
                        if (propertyCard.CurrentStockQuantity == 0)
                        {
                            // Remove property card when quantity reaches zero
                            _context.PropertyCard.Remove(propertyCard);
                        }
                        else
                        {
                            propertyCard.LastUpdated = DateTime.UtcNow;
                            _context.PropertyCard.Update(propertyCard);
                        }
                    }

                    // Check for existing PropertyAssignment
                    var existingPropertyAssignment = await _context.PropertyAssignment
                        .FirstOrDefaultAsync(pa => pa.PropertyID == detail.PropertyID && pa.EmployeeID == detail.EmployeeID);

                    if (existingPropertyAssignment != null)
                    {
                        // Update existing assignment
                        existingPropertyAssignment.DateAssigned = DateTime.Now;
                        existingPropertyAssignment.Location = employee.Office.OfficeName;
                        var previousQuantity = existingPropertyAssignment.Quantity;
                        existingPropertyAssignment.Quantity += detail.Quantity; // Add to existing quantity
                        _context.PropertyAssignment.Update(existingPropertyAssignment);

                        // Create history record for existing assignment
                        var existingHistory = new PropertyAssignmentHistory
                        {
                            PropertyID = detail.PropertyID,
                            EmployeeID = detail.EmployeeID,
                            StartDate = DateTime.UtcNow,
                            TransferType = "Transfer",
                            PropertyTransactionID = transaction.TransactionID,
                            PreviousQuantity = previousQuantity,
                            LatestQuantity = existingPropertyAssignment.Quantity,
                            Location = employee.Office.OfficeName
                        };
                        _context.PropertyAssignmentHistory.Add(existingHistory);
                    }
                    else
                    {
                        // Create new PropertyAssignment record
                        var propertyAssignment = new PropertyAssignment
                        {
                            PropertyID = detail.PropertyID,
                            EmployeeID = employee.EmployeeID,
                            DateAssigned = DateTime.Now,
                            Location = employee.Office.OfficeName,
                            Quantity = detail.Quantity
                        };
                        _context.PropertyAssignment.Add(propertyAssignment);
                        employee.IsAccountablePerson = true;

                        // Create history record for new assignment
                        var newHistory = new PropertyAssignmentHistory
                        {
                            PropertyID = detail.PropertyID,
                            EmployeeID = detail.EmployeeID,
                            StartDate = DateTime.Now,
                            TransferType = "Transfer",
                            PropertyTransactionID = transaction.TransactionID,
                            PreviousQuantity = 0, // Started with zero
                            LatestQuantity = detail.Quantity, // Received this quantity
                            Location = employee.Office.OfficeName
                        };
                        _context.PropertyAssignmentHistory.Add(newHistory);
                    }

                   
                    if (property.StockQuantity == 0)
                    {
                        // Create history record for property reaching zero
                        var propertyHistory = new PropertyAssignmentHistory
                        {
                            PropertyID = detail.PropertyID,
                            StartDate = DateTime.Now,
                            TransferType = "Transfer",
                            PropertyTransactionID = transaction.TransactionID,
                            PreviousQuantity = property.StockQuantity + detail.Quantity,
                            LatestQuantity = 0,
                            Location = "Stock"
                        };
                        _context.PropertyAssignmentHistory.Add(propertyHistory);
                    }
                    _context.Property.Update(property);

                    var transactionDetail = new PropertyTransactionDetail
                    {
                        TransactionID = transaction.TransactionID,
                        PropertyID = detail.PropertyID,
                        EmployeeID = detail.EmployeeID,
                        Quantity = detail.Quantity,
                        UnitCost = property.UnitCost,
                        TotalAmount = property.UnitCost * detail.Quantity,
                        Location = employee.Office.OfficeName,
                    };
                    _context.PropertyTransactionDetail.Add(transactionDetail);
                }

                try
                {
                    await _context.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                    TempData["SuccessMessage"] = "Properties transferred successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    await dbTransaction.RollbackAsync();
                    Console.WriteLine($"Database Error: {dbEx.Message}");
                    if (dbEx.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");
                        Console.WriteLine($"Inner Exception Stack Trace: {dbEx.InnerException.StackTrace}");
                    }
                    ModelState.AddModelError("", $"Database Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
                }
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                Console.WriteLine($"Error in TransferProperties: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
                }
                ModelState.AddModelError("", $"An error occurred: {ex.InnerException?.Message ?? ex.Message}");
            }

            propertyVM.PropertyList = new SelectList(_context.Property
                .Where(p => p.StockQuantity > 0 && p.Status != "Out of Stock")
                .Select(p => new {
                    p.PropertyID,
                    DisplayText = $"{p.PropertyName}|{p.Description}|{p.UnitMeasurement}|{p.UnitCost}"
                }), "PropertyID", "DisplayText");
            propertyVM.EmployeeList = new SelectList(_context.Employee, "EmployeeID", "EmailAddress");
            propertyVM.SupplierList = null;
            propertyVM.FundClusterList = null;
            propertyVM.OfficeList = null;
            return View(propertyVM);
        }

        [HttpGet]
        public async Task<IActionResult> RetransferProperties()
        {
            // Get all accountable employees
            var accountableEmployees = await _context.Employee
                .Where(e => e.IsAccountablePerson)
                .ToListAsync();

            // Get all employees for the transfer to dropdown
            var allEmployees = await _context.Employee.ToListAsync();

            var viewModel = new PropertyTransactionVM
            {
                // Dropdown of accountable employees
                EmployeeList = new SelectList(accountableEmployees, "EmployeeID", "FullName"),
                // Dropdown of all employees for transfer
                TransferToEmployeeList = new SelectList(allEmployees, "EmployeeID", "FullName"),
                // Will be populated via AJAX based on selected employee
                PropertyList = new SelectList(new List<Property>(), "PropertyID", "PropertyName"),
                PropertyTransactionDetail = new List<PropertyTransactionDetail>()
            };

            return View(viewModel);
        }

        // Add a new action to get assigned properties for an employee
        [HttpGet]
        public async Task<IActionResult> GetAssignedProperties(int employeeId)
        {
            var assignedProperties = await _context.PropertyAssignment
                .Include(pa => pa.Property)
                .Where(pa => pa.EmployeeID == employeeId)
                .Select(pa => new
                {
                    pa.PropertyID,
                    PropertyName = $"{pa.Property.PropertyName}, {pa.Property.Description} - {pa.Property.UnitMeasurement} (Available: {pa.Quantity})",
                    pa.Quantity
                })
                .ToListAsync();

            return Json(assignedProperties);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RetransferProperties(PropertyTransactionVM propertyVM)
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate incoming data
                if (propertyVM.EmployeeID <= 0)
                {
                    ModelState.AddModelError("", "Please select an employee to transfer from.");
                    return View(propertyVM);
                }

                if (propertyVM.TransferToEmployeeID <= 0)
                {
                    ModelState.AddModelError("", "Please select an employee to transfer to.");
                    return View(propertyVM);
                }

                if (propertyVM.PropertyTransactionDetail == null || !propertyVM.PropertyTransactionDetail.Any())
                {
                    ModelState.AddModelError("", "Please select at least one property to transfer.");
                    return View(propertyVM);
                }

                // Create transaction record
                var transaction = new PropertyTransaction
                {
                    Type = TransactionTypes.ReTransferred,
                    TransactionDate = DateTime.Now,
                    Remarks = propertyVM.Transaction?.Remarks ?? "Retransfer of property(s)"
                };
                _context.PropertyTransaction.Add(transaction);
                await _context.SaveChangesAsync();

                // Get the receiving employee with office info
                var newEmployee = await _context.Employee
                    .Include(e => e.Office)
                    .FirstOrDefaultAsync(e => e.EmployeeID == propertyVM.TransferToEmployeeID);

                if (newEmployee == null)
                {
                    await dbTransaction.RollbackAsync();
                    ModelState.AddModelError("", "Selected receiving employee not found.");
                    return View(propertyVM);
                }

                if (newEmployee.Office == null)
                {
                    await dbTransaction.RollbackAsync();
                    ModelState.AddModelError("", $"Employee {newEmployee.FullName} does not have an office assigned. Please assign an office first.");
                    return View(propertyVM);
                }

                foreach (var detail in propertyVM.PropertyTransactionDetail)
                {
                    if (detail == null) continue;

                    // Get property with its card
                    var property = await _context.Property
                        .Include(p => p.PropertyCard)
                        .FirstOrDefaultAsync(p => p.PropertyID == detail.PropertyID);

                    if (property == null)
                    {
                        await dbTransaction.RollbackAsync();
                        ModelState.AddModelError("", $"Property with ID {detail.PropertyID} not found.");
                        return View(propertyVM);
                    }

                    // Get current assignment
                    var currentAssignment = await _context.PropertyAssignment
                        .FirstOrDefaultAsync(pa => pa.PropertyID == detail.PropertyID && pa.EmployeeID == propertyVM.EmployeeID);

                    if (currentAssignment == null)
                    {
                        await dbTransaction.RollbackAsync();
                        ModelState.AddModelError("", $"Property {property.PropertyName} is not currently assigned to the selected employee.");
                        return View(propertyVM);
                    }

                    // Validate quantity
                    if (detail.Quantity <= 0)
                    {
                        await dbTransaction.RollbackAsync();
                        ModelState.AddModelError("", $"Invalid quantity for property {property.PropertyName}. Quantity must be greater than 0.");
                        return View(propertyVM);
                    }

                    if (currentAssignment.Quantity < detail.Quantity)
                    {
                        await dbTransaction.RollbackAsync();
                        ModelState.AddModelError("", $"Requested quantity ({detail.Quantity}) exceeds available quantity ({currentAssignment.Quantity}) for property {property.PropertyName}.");
                        return View(propertyVM);
                    }

                    // Update Property and PropertyCard quantities
                    var originalQuantity = property.StockQuantity;
                    property.StockQuantity = originalQuantity - detail.Quantity;
                    if (property.StockQuantity == 0)
                    {
                        property.Status = "Out of Stock";
                    }
                    _context.Property.Update(property);

                    // Update PropertyCard
                    var propertyCard = await _context.PropertyCard.FindAsync(property.PropertyCardID);
                    if (propertyCard != null)
                    {
                        var originalCardQuantity = propertyCard.CurrentStockQuantity;
                        propertyCard.CurrentStockQuantity = originalCardQuantity - detail.Quantity;
                        
                        // Only remove the property card if there are no other properties using it
                        var otherPropertiesUsingCard = await _context.Property
                            .Where(p => p.PropertyCardID == property.PropertyCardID && p.PropertyID != property.PropertyID)
                            .AnyAsync();
                            
                        if (propertyCard.CurrentStockQuantity == 0 && !otherPropertiesUsingCard)
                        {
                            // Update all properties using this card to null before removing the card
                            var propertiesUsingCard = await _context.Property
                                .Where(p => p.PropertyCardID == property.PropertyCardID)
                                .ToListAsync();
                                
                            foreach (var prop in propertiesUsingCard)
                            {
                                prop.PropertyCardID = null;
                                _context.Property.Update(prop);
                            }
                            
                            _context.PropertyCard.Remove(propertyCard);
                        }
                        else
                        {
                            propertyCard.LastUpdated = DateTime.Now;
                            _context.PropertyCard.Update(propertyCard);
                        }
                    }

                    // Update current assignment
                    currentAssignment.Quantity -= detail.Quantity;
                    if (currentAssignment.Quantity == 0)
                    {
                        // Create new history record for the old assignment with EndDate
                        var oldHistory = new PropertyAssignmentHistory
                        {
                            PropertyID = detail.PropertyID,
                            EmployeeID = propertyVM.EmployeeID,
                            StartDate = currentAssignment.DateAssigned,
                            EndDate = DateTime.UtcNow,
                            TransferType = "Retransfer",
                            PropertyTransactionID = transaction.TransactionID,
                            PreviousQuantity = currentAssignment.Quantity + detail.Quantity, // Original quantity
                            LatestQuantity = 0, // Now zero
                            Location = currentAssignment.Location
                        };
                        _context.PropertyAssignmentHistory.Add(oldHistory);

                        _context.PropertyAssignment.Remove(currentAssignment);
                        
                        // Check if employee has any remaining assignments
                        var hasRemainingAssignments = await _context.PropertyAssignment
                            .AnyAsync(pa => pa.EmployeeID == propertyVM.EmployeeID);
                        
                        if (!hasRemainingAssignments)
                        {
                            var oldEmployee = await _context.Employee
                                .FirstOrDefaultAsync(e => e.EmployeeID == propertyVM.EmployeeID);
                            if (oldEmployee != null)
                            {
                                oldEmployee.IsAccountablePerson = false;
                                _context.Employee.Update(oldEmployee);
                            }
                        }
                    }
                    else
                    {
                        // Create history record for partial transfer
                        var partialHistory = new PropertyAssignmentHistory
                        {
                            PropertyID = detail.PropertyID,
                            EmployeeID = propertyVM.EmployeeID,
                            StartDate = DateTime.Now,
                            TransferType = "Partial Retransfer",
                            PropertyTransactionID = transaction.TransactionID,
                            PreviousQuantity = currentAssignment.Quantity + detail.Quantity, // Original quantity
                            LatestQuantity = currentAssignment.Quantity, // Remaining quantity
                            Location = currentAssignment.Location
                        };
                        _context.PropertyAssignmentHistory.Add(partialHistory);
                        _context.PropertyAssignment.Update(currentAssignment);
                    }

                    // Create new assignment
                    var newAssignment = new PropertyAssignment
                    {
                        PropertyID = detail.PropertyID,
                        EmployeeID = newEmployee.EmployeeID,
                        DateAssigned = DateTime.Now,
                        Location = newEmployee.Office.OfficeName,
                        Quantity = detail.Quantity,
                        Remarks = propertyVM.Transaction?.Remarks
                    };
                    _context.PropertyAssignment.Add(newAssignment);

                    // Create new history record for the new assignment
                    var newHistory = new PropertyAssignmentHistory
                    {
                        PropertyID = detail.PropertyID,
                        EmployeeID = newEmployee.EmployeeID,
                        StartDate = DateTime.Now,
                        TransferType = "Retransfer",
                        PropertyTransactionID = transaction.TransactionID,
                        PreviousQuantity = 0, // Started with zero
                        LatestQuantity = detail.Quantity, // Received this quantity
                        Location = newEmployee.Office.OfficeName
                    };
                    _context.PropertyAssignmentHistory.Add(newHistory);

                    var transactionDetail = new PropertyTransactionDetail
                    {
                        TransactionID = transaction.TransactionID,
                        PropertyID = detail.PropertyID,
                        EmployeeID = newEmployee.EmployeeID,
                        Quantity = detail.Quantity
                    };
                    _context.PropertyTransactionDetail.Add(transactionDetail);
                }

                // Update new employee as accountable person
                newEmployee.IsAccountablePerson = true;
                _context.Employee.Update(newEmployee);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                TempData["SuccessMessage"] = "Properties retransferred successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                Console.WriteLine($"Error in RetransferProperties: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                ModelState.AddModelError("", $"An error occurred: {ex.InnerException?.Message ?? ex.Message}");
            }

            // Reload the view model data
            propertyVM.PropertyList = new SelectList(_context.Property
                .Where(p => p.StockQuantity > 0 && p.Status != "Out of Stock")
                .Select(p => new {
                    p.PropertyID,
                    DisplayText = $"{p.PropertyName}|{p.Description}|{p.UnitMeasurement}|{p.UnitCost}"
                }), "PropertyID", "DisplayText");
            propertyVM.EmployeeList = new SelectList(_context.Employee.Where(e => e.IsAccountablePerson), "EmployeeID", "FullName");
            propertyVM.TransferToEmployeeList = new SelectList(_context.Employee, "EmployeeID", "FullName");
            return View(propertyVM);
        }

        [HttpGet]
        public async Task<IActionResult> DisposeProperties()
        {
            // Get all accountable employees
            var accountableEmployees = await _context.Employee
                .Where(e => e.IsAccountablePerson)
                .ToListAsync();

            var viewModel = new PropertyTransactionVM
            {
                // Dropdown of accountable employees
                EmployeeList = new SelectList(accountableEmployees, "EmployeeID", "FullName"),
                PropertyTransactionDetail = new List<PropertyTransactionDetail>()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisposeProperties(PropertyTransactionVM propertyVM)
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get the disposal source from the form
                var disposalSource = propertyVM.DisposalSource;

                // Validate incoming data
                if (disposalSource == "assignment" && propertyVM.EmployeeID <= 0)
                {
                    ModelState.AddModelError("", "Please select an employee.");
                    return View(propertyVM);
                }

                if (string.IsNullOrWhiteSpace(propertyVM.Transaction?.Remarks))
                {
                    ModelState.AddModelError("", "Please provide a disposal reason.");
                    return View(propertyVM);
                }

                if (propertyVM.PropertyTransactionDetail == null || !propertyVM.PropertyTransactionDetail.Any())
                {
                    ModelState.AddModelError("", "Please select at least one property to dispose.");
                    return View(propertyVM);
                }

                // Create transaction record
                var transaction = new PropertyTransaction
                {
                    Type = TransactionTypes.Disposed,
                    TransactionDate = DateTime.Now,
                    Remarks = propertyVM.Transaction.Remarks
                };
                _context.PropertyTransaction.Add(transaction);
                await _context.SaveChangesAsync();

                foreach (var detail in propertyVM.PropertyTransactionDetail)
                {
                    if (detail == null) continue;

                    // Get property with its card
                    var property = await _context.Property
                        .Include(p => p.PropertyCard)
                        .FirstOrDefaultAsync(p => p.PropertyID == detail.PropertyID);

                    if (property == null)
                    {
                        await dbTransaction.RollbackAsync();
                        ModelState.AddModelError("", $"Property with ID {detail.PropertyID} not found.");
                        return View(propertyVM);
                    }

                    // If disposing from assignment, validate and update assignment
                    if (disposalSource == "assignment")
                    {
                        var currentAssignment = await _context.PropertyAssignment
                            .FirstOrDefaultAsync(pa => pa.PropertyID == detail.PropertyID && pa.EmployeeID == propertyVM.EmployeeID);

                        if (currentAssignment == null)
                        {
                            await dbTransaction.RollbackAsync();
                            ModelState.AddModelError("", $"Property {property.PropertyName} is not currently assigned to the selected employee.");
                            return View(propertyVM);
                        }

                        if (detail.Quantity <= 0)
                        {
                            await dbTransaction.RollbackAsync();
                            ModelState.AddModelError("", $"Invalid quantity for property {property.PropertyName}. Quantity must be greater than 0.");
                            return View(propertyVM);
                        }

                        if (currentAssignment.Quantity < detail.Quantity)
                        {
                            await dbTransaction.RollbackAsync();
                            ModelState.AddModelError("", $"Requested quantity ({detail.Quantity}) exceeds available quantity ({currentAssignment.Quantity}) for property {property.PropertyName}.");
                            return View(propertyVM);
                        }

                        // Update current assignment
                        currentAssignment.Quantity -= detail.Quantity;
                        if (currentAssignment.Quantity == 0)
                        {
                            // Create history record for complete disposal
                            var disposalHistory = new PropertyAssignmentHistory
                            {
                                PropertyID = detail.PropertyID,
                                EmployeeID = propertyVM.EmployeeID,
                                StartDate = currentAssignment.DateAssigned,
                                EndDate = DateTime.Now,
                                TransferType = "Disposal",
                                PropertyTransactionID = transaction.TransactionID,
                                PreviousQuantity = currentAssignment.Quantity + detail.Quantity,
                                LatestQuantity = 0,
                                Location = currentAssignment.Location
                            };
                            _context.PropertyAssignmentHistory.Add(disposalHistory);

                            _context.PropertyAssignment.Remove(currentAssignment);
                            
                            // Check if employee has any remaining assignments
                            var hasRemainingAssignments = await _context.PropertyAssignment
                                .AnyAsync(pa => pa.EmployeeID == propertyVM.EmployeeID);
                            
                            if (!hasRemainingAssignments)
                            {
                                var oldEmployee = await _context.Employee
                                    .FirstOrDefaultAsync(e => e.EmployeeID == propertyVM.EmployeeID);
                                if (oldEmployee != null)
                                {
                                    oldEmployee.IsAccountablePerson = false;
                                    _context.Employee.Update(oldEmployee);
                                }
                            }
                        }
                        else
                        {
                            // Create history record for partial disposal
                            var partialHistory = new PropertyAssignmentHistory
                            {
                                PropertyID = detail.PropertyID,
                                EmployeeID = propertyVM.EmployeeID,
                                StartDate = DateTime.Now,
                                TransferType = "Partial Disposal",
                                PropertyTransactionID = transaction.TransactionID,
                                PreviousQuantity = currentAssignment.Quantity + detail.Quantity,
                                LatestQuantity = currentAssignment.Quantity,
                                Location = currentAssignment.Location
                            };
                            _context.PropertyAssignmentHistory.Add(partialHistory);
                            _context.PropertyAssignment.Update(currentAssignment);
                        }
                    }

                    // Update property and property card
                    var originalQuantity = property.StockQuantity;
                    property.StockQuantity = originalQuantity - detail.Quantity;
                    if (property.StockQuantity == 0)
                    {
                        property.Status = "Out of Stock";
                    }
                    _context.Property.Update(property);

                    var propertyCard = await _context.PropertyCard.FindAsync(property.PropertyCardID);
                    if (propertyCard != null)
                    {
                        var originalCardQuantity = propertyCard.CurrentStockQuantity;
                        propertyCard.CurrentStockQuantity = originalCardQuantity - detail.Quantity;
                        
                        // Only remove the property card if there are no other properties using it
                        var otherPropertiesUsingCard = await _context.Property
                            .Where(p => p.PropertyCardID == property.PropertyCardID && p.PropertyID != property.PropertyID)
                            .AnyAsync();
                            
                        if (propertyCard.CurrentStockQuantity == 0 && !otherPropertiesUsingCard)
                        {
                            // Update all properties using this card to null before removing the card
                            var propertiesUsingCard = await _context.Property
                                .Where(p => p.PropertyCardID == property.PropertyCardID)
                                .ToListAsync();
                                
                            foreach (var prop in propertiesUsingCard)
                            {
                                prop.PropertyCardID = null;
                                _context.Property.Update(prop);
                            }
                            
                            _context.PropertyCard.Remove(propertyCard);
                        }
                        else
                        {
                            propertyCard.LastUpdated = DateTime.UtcNow;
                            _context.PropertyCard.Update(propertyCard);
                        }
                    }

                    // Create transaction detail
                    var transactionDetail = new PropertyTransactionDetail
                    {
                        TransactionID = transaction.TransactionID,
                        PropertyID = detail.PropertyID,
                        EmployeeID = disposalSource == "assignment" ? propertyVM.EmployeeID : null,
                        Quantity = detail.Quantity,
                        UnitCost = property.UnitCost,
                        TotalAmount = property.UnitCost * detail.Quantity,
                      
                        DisposalType = "Dispose"
                    };
                    _context.PropertyTransactionDetail.Add(transactionDetail);
                }

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                TempData["SuccessMessage"] = "Properties disposed successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                Console.WriteLine($"Error in DisposeProperties: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                ModelState.AddModelError("", $"An error occurred: {ex.InnerException?.Message ?? ex.Message}");
            }

            // Reload the view model data
            propertyVM.EmployeeList = new SelectList(_context.Employee.Where(e => e.IsAccountablePerson), "EmployeeID", "FullName");
            return View(propertyVM);
        }

        // GET: PropertyTransactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyTransaction = await _context.PropertyTransaction
                .Include(pt => pt.PropertyTransactionDetails)
                .FirstOrDefaultAsync(pt => pt.TransactionID == id);

            if (propertyTransaction == null)
            {
                return NotFound();
            }
            return View(propertyTransaction);
        }

        // POST: PropertyTransactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionID,Type,TransactionDate,Remarks")] PropertyTransaction propertyTransaction)
        {
            if (id != propertyTransaction.TransactionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(propertyTransaction);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Transaction updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyTransactionExists(propertyTransaction.TransactionID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(propertyTransaction);
        }

        // GET: PropertyTransactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyTransaction = await _context.PropertyTransaction
                .Include(pt => pt.PropertyTransactionDetails)
                .FirstOrDefaultAsync(m => m.TransactionID == id);
            if (propertyTransaction == null)
            {
                return NotFound();
            }

            return View(propertyTransaction);
        }

        // POST: PropertyTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propertyTransaction = await _context.PropertyTransaction
                .Include(pt => pt.PropertyTransactionDetails)
                .FirstOrDefaultAsync(pt => pt.TransactionID == id);

            if (propertyTransaction != null)
            {
                // Remove transaction details first
                _context.PropertyTransactionDetail.RemoveRange(propertyTransaction.PropertyTransactionDetails);
                _context.PropertyTransaction.Remove(propertyTransaction);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PropertyTransactionExists(int id)
        {
            return _context.PropertyTransaction.Any(e => e.TransactionID == id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableProperties()
        {
            var properties = await _context.Property
                .Where(p => p.StockQuantity > 0 && p.Status != "Disposed")
                .Select(p => new { 
                    p.PropertyID, 
                    p.PropertyName,
                    p.Description,
                    p.UnitMeasurement,
                    quantity = p.StockQuantity 
                })
                .ToListAsync();
            return Json(properties);
        }

    }
}
