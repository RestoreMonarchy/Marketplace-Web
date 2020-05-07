function ToggleModal(id) {
    $('#' + id).modal('toggle')
}

$(document).ready(function () {
    $("body").tooltip({ selector: '[data-toggle=tooltip]' });
});