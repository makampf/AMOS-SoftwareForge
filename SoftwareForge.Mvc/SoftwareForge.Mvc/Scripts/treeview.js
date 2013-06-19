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
        if (hiddenValue != null) {
            var path = hiddenValue.value;
            

            var link = "/TeamProjects/CodePartial?filePath=-1&guid=-2";
            link = link.replace("-1", path);
            link = link.replace("-2", ProjectGuid);
            $.get(link, function (data) {
                $('#codePartial').replaceWith(data);
            });
            
           
        }
            
    };

});



