function csheader() {

    function cshrender(selection) {
      selection.each(function(data) {
      
        var interval   = TIntervals[TPeriod];
        var format     = (interval=="month")?d3.time.format("%b %Y"):d3.time.format("%b %d %Y");
        var dateprefix = (interval=="month")?"Month of ":(interval=="week")?"Week of ":"";
        d3.select("#infodate").text(dateprefix + format(data.TIMESTAMP));
        d3.select("#infoopen").text("O " + data.OPEN);
        d3.select("#infohigh").text("H " + data.HIGH);
        d3.select("#infolow").text("L " + data.LOW);
        d3.select("#infoclose").text("C " + data.CLOSE);
    
      });
    } // cshrender
    
    return cshrender;
    } // csheader