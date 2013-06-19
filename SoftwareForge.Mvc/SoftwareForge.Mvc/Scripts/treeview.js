/*
 * Copyright (c) 2013 by Denis Bach, Marvin Kampf, Konstantin Tsysin, Taner Tunc, Florian Wittmann
 *
 * This file is part of the Software Forge Overlay rating application.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public
 * License along with this program. If not, see
 * <http://www.gnu.org/licenses/>.
 */

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



