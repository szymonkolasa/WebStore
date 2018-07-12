function openNav() {
    var sidebar = $('#sidebar');

    if (sidebar.hasClass('sidebar-visible')) {
        $('#sidebar').removeClass('sidebar-visible');
    } else {
        $('#sidebar').addClass('sidebar-visible');
    }
}

function closeNav() {
    $('#sidebar').removeClass('sidebar-visible');
}