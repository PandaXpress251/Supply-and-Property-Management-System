// SIETE - Supply and Inventory Tracking with Ease
// Main JavaScript file

$(document).ready(function () {
    // Add overlay div if it doesn't exist
    if ($('.overlay').length === 0) {
        $('#content').prepend('<div class="overlay"></div>');
    }

    // Toggle sidebar
    $('#sidebarCollapse').on('click', function (e) {
        e.preventDefault();
        $('#sidebar').toggleClass('active');
        $('.overlay').toggleClass('active');
        $('body').toggleClass('sidebar-open');

        // Save sidebar state to localStorage
        localStorage.setItem('sidebarState', $('#sidebar').hasClass('active') ? 'collapsed' : 'expanded');
    });

    // Close sidebar when clicking on overlay (mobile)
    $('.overlay').on('click', function () {
        $('#sidebar').removeClass('active');
        $('.overlay').removeClass('active');
        $('body').removeClass('sidebar-open');
    });

    // Ensure dropdown works
    $('.dropdown-toggle').on('click', function (e) {
        e.stopPropagation();
        var dropdownMenu = $(this).next('.dropdown-menu');
        $('.dropdown-menu').not(dropdownMenu).removeClass('show');
        dropdownMenu.toggleClass('show');
    });

    // Close dropdown when clicking outside
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.dropdown').length) {
            $('.dropdown-menu').removeClass('show');
        }
    });

    // Restore sidebar state from localStorage on desktop only
    if ($(window).width() > 768) {
        var sidebarState = localStorage.getItem('sidebarState');
        if (sidebarState === 'collapsed') {
            $('#sidebar').addClass('active');
        } else if (sidebarState === 'expanded') {
            $('#sidebar').removeClass('active');
        }
    }

    // Set active class to current page link
    var currentPath = window.location.pathname;

    $('.nav-link').each(function () {
        var linkPath = $(this).attr('href');

        // Check if the current path starts with the link path
        // This ensures parent links are highlighted when child pages are active
        if (currentPath === linkPath ||
            (linkPath !== '/' && currentPath.startsWith(linkPath))) {
            $(this).addClass('active');

            // Expand parent category if collapsed
            $(this).parents('.nav-category').addClass('show');
        }
    });

    // Initialize any DataTables on the page
    if ($.fn.DataTable) {
        $('.data-table').DataTable({
            responsive: true,
            language: {
                search: "_INPUT_",
                searchPlaceholder: "Search...",
                lengthMenu: "Show _MENU_ entries",
                info: "Showing _START_ to _END_ of _TOTAL_ entries",
                infoEmpty: "Showing 0 to 0 of 0 entries",
                infoFiltered: "(filtered from _MAX_ total entries)"
            }
        });
    }

    // Handle form validation styling
    $('.needs-validation').on('submit', function (event) {
        if (!this.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        }

        $(this).addClass('was-validated');
    });

    // Add responsive behavior to tables
    $('.table').addClass('table-responsive-sm');

    // Add smooth scrolling to page
    $(document).on('click', 'a.smooth-scroll', function (event) {
        if (this.hash !== "") {
            event.preventDefault();
            var hash = this.hash;

            $('html, body').animate({
                scrollTop: $(hash).offset().top - 70
            }, 800, function () {
                window.location.hash = hash;
            });
        }
    });

    // Handle category collapse in sidebar
    $('.category-title').on('click', function () {
        $(this).siblings('.nav-items').slideToggle(300);
        $(this).parent().toggleClass('collapsed');
    });

    // Handle window resize - reset sidebar on mobile/desktop switch
    $(window).resize(function () {
        if ($(window).width() <= 768) {
            $('#sidebar').removeClass('active');
            $('.overlay').removeClass('active');
        } else {
            // On desktop, restore saved state
            var sidebarState = localStorage.getItem('sidebarState');
            if (sidebarState === 'collapsed') {
                $('#sidebar').addClass('active');
            } else if (sidebarState === 'expanded') {
                $('#sidebar').removeClass('active');
            }
        }
    });
});