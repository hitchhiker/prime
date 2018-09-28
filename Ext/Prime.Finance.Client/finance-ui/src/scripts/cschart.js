function cschart() {

    var margin = {top: 0, right: 30, bottom: 40, left: 5},
        width = 620, height = 300, Bheight = 460;

    function csrender(selection) {
      selection.each(function() {
     
        var interval = TIntervals[TPeriod];

        var minimal  = d3.min(genData, function(d) { return d.LOW; });
        var maximal  = d3.max(genData, function(d) { return d.HIGH; });

        var extRight = width + margin.right
        var x = d3.scale.ordinal()
            .rangeBands([0, width]);
        
        var y = d3.scale.linear()
            .rangeRound([height, 0]);
        
        var xAxis = d3.svg.axis()
            .scale(x)
            .tickFormat(d3.time.format(TFormat[interval]));
        
        var yAxis = d3.svg.axis()
            .scale(y)
            .ticks(Math.floor(height/50));

        x.domain(genData.map(function(d) { return d.TIMESTAMP; }));
        y.domain([minimal, maximal]).nice();
    
        var xtickdelta   = Math.ceil(60/(width/genData.length))
        xAxis.tickValues(x.domain().filter(function(d, i) { return !((i+Math.floor(xtickdelta/2)) % xtickdelta); }));
    
        var barwidth    = x.rangeBand();
        var candlewidth = Math.floor(d3.min([barwidth*0.8, 13])/2)*2+1;
        var delta       = Math.round((barwidth-candlewidth)/2);
    
        d3.select(this).select("svg").remove();
        var svg = d3.select(this).append("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", Bheight + margin.top + margin.bottom)
          .append("g")
            .attr("transform", "translate(" + margin.left + "," + margin.top + ")");
        
        svg.append("g")
            .attr("class", "axis xaxis")
            .attr("transform", "translate(0," + height + ")")
            .call(xAxis.orient("bottom").outerTickSize(0));
    
        svg.append("g")
            .attr("class", "axis yaxis")
            .attr("transform", "translate(" + width + ",0)")
            .call(yAxis.orient("right").tickSize(0));
    
        svg.append("g")
            .attr("class", "axis grid")
            .attr("transform", "translate(" + width + ",0)")
            .call(yAxis.orient("left").tickFormat("").tickSize(width).outerTickSize(0));
    
        var bands = svg.selectAll(".bands")
            .data([genData])
          .enter().append("g")
            .attr("class", "bands");
    
        bands.selectAll("rect")
            .data(function(d) { return d; })
          .enter().append("rect")
            .attr("x", function(d) { return x(d.TIMESTAMP) + Math.floor(barwidth/2); })
            .attr("y", 0)
            .attr("height", Bheight)
            .attr("width", 1)
            .attr("class", function(d, i) { return "band"+i; })
            .style("stroke-width", Math.floor(barwidth));
    
        var stick = svg.selectAll(".sticks")
            .data([genData])
          .enter().append("g")
            .attr("class", "sticks");
    
        stick.selectAll("rect")
            .data(function(d) { return d; })
          .enter().append("rect")
            .attr("x", function(d) { return x(d.TIMESTAMP) + Math.floor(barwidth/2); })
            .attr("y", function(d) { return y(d.HIGH); })
            .attr("class", function(d, i) { return "stick"+i; })
            .attr("height", function(d) { return y(d.LOW) - y(d.HIGH); })
            .attr("width", 1)
            .classed("rise", function(d) { return (d.CLOSE>d.OPEN); })
            .classed("fall", function(d) { return (d.OPEN>d.CLOSE); });
    
        var candle = svg.selectAll(".candles")
            .data([genData])
          .enter().append("g")
            .attr("class", "candles");
    
        candle.selectAll("rect")
            .data(function(d) { return d; })
          .enter().append("rect")
            .attr("x", function(d) { return x(d.TIMESTAMP) + delta; })
            .attr("y", function(d) { return y(d3.max([d.OPEN, d.CLOSE])); })
            .attr("class", function(d, i) { return "candle"+i; })
            .attr("height", function(d) { return y(d3.min([d.OPEN, d.CLOSE])) - y(d3.max([d.OPEN, d.CLOSE])); })
            .attr("width", candlewidth)
            .classed("rise", function(d) { return (d.CLOSE>d.OPEN); })
            .classed("fall", function(d) { return (d.OPEN>d.CLOSE); });

      });
    } // csrender

    csrender.Bheight = function(value) {
            	if (!arguments.length) return Bheight;
            	Bheight = value;
            	return csrender;
        	};
  
return csrender;
} // cschart