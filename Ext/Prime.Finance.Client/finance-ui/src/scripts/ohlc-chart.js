window.onload = function () {
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
            dragmode: 'pan',
            showlegend: false,
            height: 400,
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

        Plotly.plot('plotly-div', data, layout, {scrollZoom: true, responsive: true});
    });
};

