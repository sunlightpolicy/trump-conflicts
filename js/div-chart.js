/**
 * Concrete row chart implementation.
 *
 * Examples:
 * - {@link http://dc-js.github.com/dc.js/ Nasdaq 100 Index}
 * @class rowChart
 * @memberof dc
 * @mixes dc.capMixin
 * @mixes dc.marginMixin
 * @mixes dc.colorMixin
 * @mixes dc.baseMixin
 * @example
 * // create a row chart under #chart-container1 element using the default global chart group
 * var chart1 = dc.rowChart('#chart-container1');
 * // create a row chart under #chart-container2 element using chart group A
 * var chart2 = dc.rowChart('#chart-container2', 'chartGroupA');
 * @param {String|node|d3.selection} parent - Any valid
 * {@link https://github.com/d3/d3-3.x-api-reference/blob/master/Selections.md#selecting-elements d3 single selector} specifying
 * a dom block element such as a div; or a dom element or d3 selection.
 * @param {String} [chartGroup] - The name of the chart group this chart instance should be placed in.
 * Interaction with a chart will only trigger events and redraws within the chart's group.
 * @returns {dc.rowChart}
 */
dc.divChart = function (parent, chartGroup) {


    var _rowCssClass = 'row';
    var _chart = dc.capMixin(dc.marginMixin(dc.colorMixin(dc.baseMixin({}))));

    _chart.rowsCap = _chart.cap;


    _chart._doRender = function () {
        drawChart();
        return _chart;
    };

    function drawChart () {
        drawDivs(_chart.data());
    }

    function drawDivs (data) {

        _chart.select('div').remove();
        _chart.select('span').remove();
        
        _chart.root()
            .append('span')
            .attr('class', 'chart-title')
            .text('Filter by Family Member');

        var div = _chart.root()
            .append('div')
            .attr('class', "div-chart"); 

        //var divs = div.selectAll('g.' + _rowCssClass)
        var divs = div.selectAll('X')
            .data(data);

        divs.enter()
            .append('div')
            .attr('class', function (d, i) {
                return "div-chart-item " + i;
            })
            .on('click', onClick)
            .classed('deselected', d => !isSelectedRow(d) && _chart.hasFilter())
            .classed('selected', d => isSelectedRow(d) && _chart.hasFilter())
            .classed('noneSelected', d => !_chart.hasFilter())
            .html( function (d) {
                return divHtml(d);
            });
    }

    function isSelectedRow (d) {
        return _chart.hasFilter(_chart.cappedKeyAccessor(d));
    }

    function divHtml(d) {
        let familyMember = d.key;

        let style = getComputedStyle(document.body);
        let phoneWidth = style.getPropertyValue('--phone-width');
        let isPhone = window.matchMedia("screen and (max-width: " + phoneWidth + ")").matches;

        // Ugh..
        var imageWidth = "115";
        var imageHeight = "140";
        if (isPhone) {
            imageWidth = "80";
            imageHeight = "98";
        }

        let html =      
            '<h3 class="div-chart-header">' + familyMember + '</h3>' +
            '<p>' + d.value + ' potential conflicts</p>' +
            '<img class="" dsrc="blank.gif" data-original="img/' + familyMember + '.jpg" ' +
            'alt="' + familyMember + '" ' +
            'title="' + familyMember + '" ' +
            'width="' + imageWidth + '" ' +
            'height="' + imageHeight + '" ' +
            'src="img/' + familyMember + '.jpg" style="display: inline;">';
        return html
    }

    function onClick (d) {
        _chart.onClick(d);
    }

    _chart._doRedraw = function () {
        drawChart();
        return _chart;
    };

    return _chart.anchor(parent, chartGroup);
};
