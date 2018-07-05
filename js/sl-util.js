function makeTable (cols, data) {

    head = '';
    for(i = 0; i < cols.length; i++)
        head += th(cols[i].header, cols[i].class);
    head = thead(head);    

    body = ''; 
    data.forEach(row => {
        var tds = ""
        cols.forEach(col => {
            var txt = row[col.field];
            var cssClass = col.class;
            if (cssClass == "td-percent")
                txt += "%";

            tds += td(txt, cssClass);
        });
        body += tr(tds);     
    });
    body = tbody(body);
    
    var s = table(head + body);
    console.log(s); 
    
    return table(s);
}


function tbody(s) {
    return "<tbody>" + s + "</tbody>";
}

function thead(s) {
    return "<thead>" + s + "</thead>";
}

function th(s, cls) {
    var css = ""
    if (cls)
        css = " class='" + cls + "'";
    return "<th" + css + ">" + s + "</th>";
}

function td(s, cls) {
    var css = ""
    if (cls)
        css = " class='" + cls + "'";
    return "<td" + css + ">" + s + "</td>";
}

function tr(s) {
    return "<tr>" + s + "</tr>";
} 

function table(s) {
    return "<table>" + s + "</table>";
    //return "<table border=1>" + s + "</table>";
} 

function h2(s) {
    return "<h2>" + s + "</h2>";
}

function h3(s) {
    return "<h3>" + s + "</h3>";
}

function h4(s) {
    return "<h4>" + s + "</h4>";
}

function p(s) {
    return "<p>" + s + "</p>";
}