// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function todoItemIsDoneCheckbox() {
    var hideDoneItemsFromUser = $("#isDoneCheckbox").prop('checked');
    $('li[name="todoItem"]').each(function () {
        var isItemDone = this.getAttribute('data-isdone').toLowerCase() == 'true';
        if (isItemDone && hideDoneItemsFromUser) {
            $(this).hide(200);
        } else {
            $(this).show(200);
        }
    });
} 