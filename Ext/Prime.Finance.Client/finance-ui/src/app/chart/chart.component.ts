import { Component, OnInit } from '@angular/core';
import * as d3 from "d3";
import { HttpClient } from '@angular/common/http';
import { OhlcRawResponse } from './ohlc/ohlc-raw-response';
import { map } from 'rxjs/operators';
import { OhlcRecord } from './ohlc/ohlc-record';
import { range } from 'rxjs';

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.scss']
})
export class ChartComponent implements OnInit {

  constructor(
    private httpClient: HttpClient
  ) { }

  ngOnInit() {
    this.httpClient.get("/assets/ohlc.json").pipe(map((o) => {
      let rRaw = (<OhlcRawResponse>o).result["XXBTZUSD"];
      let ohlcs: OhlcRecord[] = rRaw.map((v) => {
        return {
          time: v[0],
          open: v[1],
          high: v[2],
          low: v[3],
          close: v[4],
          vwap: v[5],
          volume: v[6],
          count: v[7]
        };;
      })

      return ohlcs.sort((a, b) => {
        return a.time > b.time ? 1 : -1;
      });
    })).subscribe((d) => {
      this.draw(d);
    });
  }

  private draw(d: OhlcRecord[]) {
    // "https://api.kraken.com/0/public/OHLC?pair=XBTUSD"
    let xPos: number = 0;
    let svgHeight = 450;

    let yScaleMin: number = d3.min(d, (d: OhlcRecord) => {
      return d.low;
    });

    let yScaleMax: number = d3.max(d, (d: OhlcRecord) => {
      return d.high;
    });


    let yScaleRaw = d3.scaleLinear()
      .domain([yScaleMin, yScaleMax])
      .range([0, svgHeight]);
    let yScale = function(v) {
        return yScaleRaw(v).toFixed(4);
    };


    let svg = d3.select("#plotly-div")
      .append("svg").attr("width", "100%").attr("height", svgHeight);

    let data = svg.selectAll("rect")
      .data(d)

    let groups = data.enter().append("g")
      .attr("transform", (x: OhlcRecord, i) => {
        return `translate(${i * 10}, ${yScale(x.low)})`;
      });

    groups.append("line");
    groups.append("rect");

    groups.selectAll("line")
      .attr("transform", (x: OhlcRecord, i) => {
        return `translate(${4}, ${0})`;
      })
      .attr("x1", 0).attr("x2", 0)
      .attr("y1", 0)
      .attr("y2", (x: OhlcRecord, i) => {
        return (yScale(x.high) - yScale(x.low));
      })
      .attr("stroke-width", 1)
      .attr("stroke", (o: OhlcRecord) => {
        return getColor(o);
      })
      .exit().remove();

    groups.selectAll("rect")
      .attr("width", 9)
      .attr("height", (o: OhlcRecord) => {
        return Math.abs(yScale(o.open) - yScale(o.close);
      })
      .attr("fill", (o: OhlcRecord) => {
        return getColor(o);
      })
      .attr("x", 0)
      .attr("y", (x: OhlcRecord, i) => {
        let openLow = yScale(x.open) - yScale(x.low);
        let closeLow = yScale(x.close) - yScale(x.low)
        return openLow < closeLow ? openLow : closeLow;
      }).exit().remove();

    function getColor(o: OhlcRecord): string {
      return o.open - o.close >= 0 ? "red" : "green";
    }

    // d3.json("", function (data) {
    //   console.log(data[0]);
    // });


    //svg.append("rect").attr("width", 100).attr("height", 50).attr("fill", "red").attr("x", 50).attr("y", 100);
  }
}
