$(function () {
    $('form').submit(function (e) {
        e.preventDefault();
        var query = $('#query').val();
        $('tbody').load('/Users/SearchUser?query=' + query);
    });

});