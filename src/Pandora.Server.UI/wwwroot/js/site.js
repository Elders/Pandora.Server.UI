// Write your Javascript code.

function editSetting(key) {

    $('#setting-row-' + key + ' .edit').hide();
    $('#setting-row-' + key + ' .delete').hide();
    $('#setting-row-' + key + ' .save').show();
    $('#setting-row-' + key + ' .cancel').show();

    $('span.setting-key-' + key).addClass('hidden');
    $('input.setting-key-' + key).removeClass('hidden');

    $('span.setting-value-' + key).addClass('hidden');
    $('input.setting-value-' + key).removeClass('hidden');

    var settingKey = $('span.setting-key-' + key).text();
    $('input.setting-key-' + key).val(settingKey);

    var settingValue = $('span.setting-value-' + key).text();
    $('input.setting-value-' + key).val(settingValue);
}

function saveChanges(key) {

    $('#setting-row-' + key + ' .edit').show();
    $('#setting-row-' + key + ' .delete').show();
    $('#setting-row-' + key + ' .save').hide();
    $('#setting-row-' + key + ' .cancel').hide();

    $('span.setting-key-' + key).removeClass('hidden');
    $('input.setting-key-' + key).addClass('hidden');

    $('span.setting-value-' + key).removeClass('hidden');
    $('input.setting-value-' + key).addClass('hidden');

    var settingKey = $('input.setting-key-' + key).val();
    if (settingKey) {
        $('input.setting-value-' + key).attr('name', 'config[' + settingKey + ']');
        $('span.setting-key-' + key).text(settingKey);
    }

    var settingValue = $('input.setting-value-' + key).val();
    if (settingValue) {
        $('span.setting-value-' + key).text(settingValue);
    }
}

function deleteSetting(key) {
    if (confirm('Delete ' + key + '?')) {
        $('#setting-row-' + key).remove();
    }
}

function cancelEdit(key) {

    $('#setting-row-' + key + ' .edit').show();
    $('#setting-row-' + key + ' .delete').show();
    $('#setting-row-' + key + ' .save').hide();
    $('#setting-row-' + key + ' .cancel').hide();

    $('span.setting-key-' + key).removeClass('hidden');
    $('input.setting-key-' + key).addClass('hidden');

    $('span.setting-value-' + key).removeClass('hidden');
    $('input.setting-value-' + key).addClass('hidden');
}