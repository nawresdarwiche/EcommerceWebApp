functionconfirmDelete(uniqueId, isDeleteClicked) {
    vardeleteSpan = 'deleteSpan_' + uniqueId;
    varconfirmDeleteSpan = 'confirmDeleteSpan_' + uniqueId;
    if (isDeleteClicked) {
        $('#' + deleteSpan).hide();
        $('#' + confirmDeleteSpan).show();
    } else {
        $('#' + deleteSpan).show();
        $('#' + confirmDeleteSpan).hide();
    }
}
