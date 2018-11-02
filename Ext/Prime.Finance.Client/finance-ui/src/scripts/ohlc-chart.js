window.onload = function () {
    return;
    var margin = { top: 20, right: 20, bottom: 30, left: 50 },
        width = 1000 - margin.left - margin.right,
        height = 450 - margin.top - margin.bottom;

    var parseDate = d3.timeParse("%d-%b-%y");

    var x = techan.scale.financetime()
        .range([0, width]);

    var y = d3.scaleLinear()
        .range([height, 0]);

    var candlestick = techan.plot.candlestick()
        .xScale(x)
        .yScale(y);

    var xAxis = d3.axisBottom()
        .scale(x);

    var yAxis = d3.axisLeft()
        .scale(y);

    var svg = d3.select("#plotly-div").append("svg")
        .attr("width", "100%")
        //.attr("width", width + margin.left + margin.right)
        .attr("height", "100%")
        //.attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    d3.csv("assets/data.csv", function (error, data) {
        var accessor = candlestick.accessor();

        data = data.slice(0, 200).map(function (d) {
            return {
                date: parseDate(d.Date),
                open: +d.Open,
                high: +d.High,
                low: +d.Low,
                close: +d.Close,
                volume: +d.Volume
            };
        }).sort(function (a, b) { return d3.ascending(accessor.d(a), accessor.d(b)); });

        svg.append("g")
            .attr("class", "candlestick");

        svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + height + ")");

        svg.append("g")
            .attr("class", "y axis")
            .append("text")
            .attr("transform", "rotate(-90)")
            .attr("y", 6)
            .attr("dy", ".71em")
            .style("text-anchor", "end")
            .text("Price ($)");

        // Data to display initially
        draw(data.slice(0, data.length - 20));
        // Only want this button to be active if the data has loaded
        d3.select("button").on("click", function () { draw(data); }).style("display", "inline");
    });

    function draw(data) {
        x.domain(data.map(candlestick.accessor().d));
        y.domain(techan.scale.plot.ohlc(data, candlestick.accessor()).domain());

        svg.selectAll("g.candlestick").datum(data).call(candlestick);
        svg.selectAll("g.x.axis").call(xAxis);
        svg.selectAll("g.y.axis").call(yAxis);
    }



    return;
    Plotly.d3.csv('https://raw.githubusercontent.com/plotly/datasets/master/finance-charts-apple.csv', function (err, rows) {

        function unpack(rows, key) {
            return rows.map(function (row) {
                return row[key];
            });
        }

        var trace = {
            x: unpack(rows, 'Date'),
            close: unpack(rows, 'AAPL.Close'),
            high: unpack(rows, 'AAPL.High'),
            low: unpack(rows, 'AAPL.Low'),
            open: unpack(rows, 'AAPL.Open'),

            // cutomise colors 
            increasing: { line: { color: '#38bf37' } },
            decreasing: { line: { color: '#e82222' } },

            type: 'candlestick',
            xaxis: 'x',
            yaxis: 'y',

            hoverlabel: {
                font: {
                    family: "sans-serif"
                }
            }
        };

        var data = [trace];

        var layout = {
            autosize: true,
            dragmode: 'pan',
            showlegend: false,
            paper_bgcolor: "#182e42",
            plot_bgcolor: "transparent",
            font: {
                color: "white"
            },
            xaxis: {
                autorange: true,
                title: 'Date',
            },
            yaxis: {
                autorange: true,
            }
        };

        Plotly.plot('plotly-div', data, layout, { scrollZoom: true, responsive: true });
    });
};

