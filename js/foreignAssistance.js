var billion = 1000000000;

var appropriationTypeColors =
    ["#74C365", // light green 
    "#006600",  // dark green 
    "#007BA7"]; // blue


d3.csv("data/foreignAssistance.csv", function (data) {
    data.forEach(function (d) {
        d.amount = +d.amount; // cast to numbers
    });
    // put data in crossfilter
    var facts = crossfilter(data);

    // draw all dc charts. w/o this nothing happens!  
    dc.renderAll();
});
