// Global Modal Module
const GlobalModal = (function() {
    'use strict';

    // Private variables
    const modalId = '#globalModal';
    const modalBodyId = '#globalModalBody';
    const modalLabelId = '#globalModalLabel';

    // Private methods
    function showLoadingState() {
        const loadingHTML = `
            <div class="d-flex align-items-center justify-content-center py-5">
                <div class="spinner-border text-primary me-2" role="status"></div>
                <strong>Loading content...</strong>
            </div>`;
        $(modalBodyId).html(loadingHTML);
    }

    function showSuccessToast(message) {
        const toast = `
            <div class="toast-container position-fixed bottom-0 end-0 p-3">
                <div class="toast align-items-center text-white bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="d-flex">
                        <div class="toast-body">
                            <i class="fas fa-check-circle me-2"></i>
                            ${message}
                        </div>
                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                    </div>
                </div>
            </div>`;
        
        $('.toast-container').remove();
        $('body').append(toast);
        
        const toastElement = $('.toast');
        const bsToast = new bootstrap.Toast(toastElement, {
            autohide: true,
            delay: 500
        });
        bsToast.show();
    }

    function refreshPage() {
        // Destroy any DataTables instances
        if ($.fn.DataTable.isDataTable('#employeesTable')) {
            $('#employeesTable').DataTable().destroy();
        }
        
        // Force a hard refresh
        window.location.reload(true);
    }

    function handleFormSubmit(e) {
        e.preventDefault();

        const form = $(this);
        const submitBtn = form.find('button[type="submit"]');

        // Disable submit button and show loading state
        submitBtn.prop('disabled', true)
            .html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Saving...');

        $.ajax({
            url: form.attr("action"),
            method: form.attr("method"),
            data: form.serialize(),
            success: function(result) {
                if (typeof result === 'object' && result.success) {
                    $(modalId).modal('hide');
                    showSuccessToast(result.message || 'Changes saved successfully!');
                    setTimeout(refreshPage, 500);
                } else {
                    const hasErrors = $(result).find(".field-validation-error").length > 0;
                    if (hasErrors) {
                        $(modalBodyId).html(result);
                    } else {
                        $(modalBodyId).html('<div class="alert alert-danger">An unexpected error occurred.</div>');
                    }
                }
            },
            error: function(xhr) {
                if (xhr.status === 400) {
                    $(modalBodyId).html(xhr.responseText);
                } else {
                    $(modalBodyId).html('<div class="alert alert-danger">An error occurred while submitting the form.</div>');
                }
            },
            complete: function() {
                submitBtn.prop('disabled', false)
                    .html('<i class="fas fa-save me-2"></i>Save Changes');
            }
        });
    }

    // Public methods
    return {
        init: function() {
            // Initialize form submission handler
            $(document).on("submit", `${modalId} form`, handleFormSubmit);
        },

        open: function(title, url) {
            $(modalLabelId).text(title);
            showLoadingState();
            $(modalId).modal('show');

            $.get(url)
                .done(function(data) {
                    $(modalBodyId).html(data);
                })
                .fail(function() {
                    $(modalBodyId).html('<div class="alert alert-danger">Failed to load content.</div>');
                });
        },

        close: function() {
            $(modalId).modal('hide');
            $(modalId).on('hidden.bs.modal', function() {
                showLoadingState();
            });
        }
    };
})();

// Initialize when document is ready
$(document).ready(function() {
    GlobalModal.init();
});

// Make openGlobalModal available globally
window.openGlobalModal = GlobalModal.open;
window.closeGlobalModal = GlobalModal.close;
