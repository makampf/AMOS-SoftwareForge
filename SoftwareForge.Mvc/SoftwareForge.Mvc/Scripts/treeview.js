$(document).ready(function () {
    // Create tree view
    $("#browser").treeview();
    
    
    // Gets the event target in a browser-compatible way
    function getEventTarget(e) {
        e = e || window.event;
        return e.target || e.srcElement;
    }

    //register on click - event
    var ul = document.getElementById('browser');
    ul.onclick = function (event) {
        var target = getEventTarget(event);
        var hiddenValue = target.attributes.getNamedItem("hidden");
        if (hiddenValue != null)
            alert(hiddenValue.value);
    };

});



