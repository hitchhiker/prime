var parseDate = d3.time.format("%Y-%m-%d").parse;
var TPeriod = "3M";
var TDays = { "1M": 21, "3M": 63, "6M": 126, "1Y": 252, "2Y": 504, "4Y": 1008 };
var TIntervals = { "1M": "day", "3M": "day", "6M": "day", "1Y": "week", "2Y": "week", "4Y": "month" };
var TFormat = { "day": "%d %b '%y", "week": "%d %b '%y", "month": "%b '%y" };
var genRaw, genData;

test();


function test() {
    var trace1 = {
        x: ['2017-01-04', '2017-01-05', '2017-01-06', '2017-01-09', '2017-01-10', '2017-01-11', '2017-01-12', '2017-01-13', '2017-01-17', '2017-01-18', '2017-01-19', '2017-01-20', '2017-01-23', '2017-01-24', '2017-01-25', '2017-01-26', '2017-01-27', '2017-01-30', '2017-01-31', '2017-02-01', '2017-02-02', '2017-02-03', '2017-02-06', '2017-02-07', '2017-02-08', '2017-02-09', '2017-02-10', '2017-02-13', '2017-02-14', '2017-02-15'],
        close: [116.019997, 116.610001, 117.910004, 118.989998, 119.110001, 119.75, 119.25, 119.040001, 120, 119.989998, 119.779999, 120, 120.080002, 119.970001, 121.879997, 121.940002, 121.949997, 121.629997, 121.349998, 128.75, 128.529999, 129.080002, 130.289993, 131.529999, 132.039993, 132.419998, 132.119995, 133.289993, 135.020004, 135.509995],
        decreasing: { line: { color: '#7F7F7F' } },
        high: [116.510002, 116.860001, 118.160004, 119.43, 119.379997, 119.93, 119.300003, 119.620003, 120.239998, 120.5, 120.089996, 120.449997, 120.809998, 120.099998, 122.099998, 122.440002, 122.349998, 121.629997, 121.389999, 130.490005, 129.389999, 129.190002, 130.5, 132.089996, 132.220001, 132.449997, 132.940002, 133.820007, 135.089996, 136.270004],
        increasing: { line: { color: '#17BECF' } },
        line: { color: 'rgba(31,119,180,1)' },
        low: [115.75, 115.809998, 116.470001, 117.940002, 118.300003, 118.599998, 118.209999, 118.809998, 118.220001, 119.709999, 119.370003, 119.730003, 119.769997, 119.5, 120.279999, 121.599998, 121.599998, 120.660004, 120.620003, 127.010002, 127.779999, 128.160004, 128.899994, 130.449997, 131.220001, 131.119995, 132.050003, 132.75, 133.25, 134.619995],
        open: [115.849998, 115.919998, 116.779999, 117.949997, 118.769997, 118.739998, 118.900002, 119.110001, 118.339996, 120, 119.400002, 120.449997, 120, 119.550003, 120.419998, 121.669998, 122.139999, 120.93, 121.150002, 127.029999, 127.980003, 128.309998, 129.130005, 130.539993, 131.350006, 131.649994, 132.460007, 133.080002, 133.470001, 135.520004],
        type: 'candlestick',
        xaxis: 'x',
        yaxis: 'y'
    };

    var data = [trace1];

    var layout = {
        dragmode: 'zoom',
        margin: {
            r: 10,
            t: 25,
            b: 40,
            l: 60
        },
        showlegend: false,
        xaxis: {
            autorange: true,
            domain: [0, 1],
            range: ['2017-01-03 12:00', '2017-02-15 12:00'],
            rangeslider: { range: ['2017-01-03 12:00', '2017-02-15 12:00'] },
            title: 'Date',
            type: 'date'
        },
        yaxis: {
            autorange: true,
            domain: [0, 1],
            range: [114.609999778, 137.410004222],
            type: 'linear'
        }
    };

    Plotly.plot('plotly-div', data, layout);
}

(function () {
    d3.csv("assets/stockdata.csv", genType, function (data) {
        console.log(data);
        genRaw = data;
        mainjs();
    });
}());

function toSlice(data) { return data.slice(-TDays[TPeriod]); }

function mainjs() {
    var toPress = function () { genData = (TIntervals[TPeriod] != "day") ? dataCompress(toSlice(genRaw), TIntervals[TPeriod]) : toSlice(genRaw); };
    toPress(); displayAll();
    d3.select("#oneM").on("click", function () { TPeriod = "1M"; toPress(); displayAll(); });
    d3.select("#threeM").on("click", function () { TPeriod = "3M"; toPress(); displayAll(); });
    d3.select("#sixM").on("click", function () { TPeriod = "6M"; toPress(); displayAll(); });
    d3.select("#oneY").on("click", function () { TPeriod = "1Y"; toPress(); displayAll(); });
    d3.select("#twoY").on("click", function () { TPeriod = "2Y"; toPress(); displayAll(); });
    d3.select("#fourY").on("click", function () { TPeriod = "4Y"; toPress(); displayAll(); });
}

function displayAll() {
    changeClass();
    displayCS();
    displayGen(genData.length - 1);
}

function changeClass() {
    if (TPeriod == "1M") {
        d3.select("#oneM").classed("active", true);
        d3.select("#threeM").classed("active", false);
        d3.select("#sixM").classed("active", false);
        d3.select("#oneY").classed("active", false);
        d3.select("#twoY").classed("active", false);
        d3.select("#fourY").classed("active", false);
    } else if (TPeriod == "6M") {
        d3.select("#oneM").classed("active", false);
        d3.select("#threeM").classed("active", false);
        d3.select("#sixM").classed("active", true);
        d3.select("#oneY").classed("active", false);
        d3.select("#twoY").classed("active", false);
        d3.select("#fourY").classed("active", false);
    } else if (TPeriod == "1Y") {
        d3.select("#oneM").classed("active", false);
        d3.select("#threeM").classed("active", false);
        d3.select("#sixM").classed("active", false);
        d3.select("#oneY").classed("active", true);
        d3.select("#twoY").classed("active", false);
        d3.select("#fourY").classed("active", false);
    } else if (TPeriod == "2Y") {
        d3.select("#oneM").classed("active", false);
        d3.select("#threeM").classed("active", false);
        d3.select("#sixM").classed("active", false);
        d3.select("#oneY").classed("active", false);
        d3.select("#twoY").classed("active", true);
        d3.select("#fourY").classed("active", false);
    } else if (TPeriod == "4Y") {
        d3.select("#oneM").classed("active", false);
        d3.select("#threeM").classed("active", false);
        d3.select("#sixM").classed("active", false);
        d3.select("#oneY").classed("active", false);
        d3.select("#twoY").classed("active", false);
        d3.select("#fourY").classed("active", true);
    } else {
        d3.select("#oneM").classed("active", false);
        d3.select("#threeM").classed("active", true);
        d3.select("#sixM").classed("active", false);
        d3.select("#oneY").classed("active", false);
        d3.select("#twoY").classed("active", false);
        d3.select("#fourY").classed("active", false);
    }
}

function displayCS() {
    var chart = cschart().Bheight(460);
    d3.select("#chart1").call(chart);
    var chart = barchart().mname("volume").margin(320).MValue("TURNOVER");
    d3.select("#chart1").datum(genData).call(chart);
    var chart = barchart().mname("sigma").margin(400).MValue("VOLATILITY");
    d3.select("#chart1").datum(genData).call(chart);
    hoverAll();
}

function hoverAll() {
    d3.select("#chart1").select(".bands").selectAll("rect")
        .on("mouseover", function (d, i) {
            d3.select(this).classed("hoved", true);
            d3.select(".stick" + i).classed("hoved", true);
            d3.select(".candle" + i).classed("hoved", true);
            d3.select(".volume" + i).classed("hoved", true);
            d3.select(".sigma" + i).classed("hoved", true);
            displayGen(i);
        })
        .on("mouseout", function (d, i) {
            d3.select(this).classed("hoved", false);
            d3.select(".stick" + i).classed("hoved", false);
            d3.select(".candle" + i).classed("hoved", false);
            d3.select(".volume" + i).classed("hoved", false);
            d3.select(".sigma" + i).classed("hoved", false);
            displayGen(genData.length - 1);
        });
}

function displayGen(mark) {
    var header = csheader();
    d3.select("#infobar").datum(genData.slice(mark)[0]).call(header);
}