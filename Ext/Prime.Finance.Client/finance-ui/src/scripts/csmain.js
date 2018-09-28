var parseDate = d3.time.format("%Y-%m-%d").parse;
var TPeriod = "3M";
var TDays = { "1M": 21, "3M": 63, "6M": 126, "1Y": 252, "2Y": 504, "4Y": 1008 };
var TIntervals = { "1M": "day", "3M": "day", "6M": "day", "1Y": "week", "2Y": "week", "4Y": "month" };
var TFormat = { "day": "%d %b '%y", "week": "%d %b '%y", "month": "%b '%y" };
var genRaw, genData;

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