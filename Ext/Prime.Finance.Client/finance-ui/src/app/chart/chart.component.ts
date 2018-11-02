import { Component, OnInit } from '@angular/core';
import * as d3 from "d3";
import { HttpClient } from '@angular/common/http';
import { OhlcRawResponse } from './ohlc/ohlc-raw-response';
import { map } from 'rxjs/operators';
import { OhlcRecord } from './ohlc/ohlc-record';

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

      return ohlcs;
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
    })

    var yScale = d3.scaleLinear()
      .domain([yScaleMin, yScaleMax])
      .range([0, svgHeight]);

    console.log({yScaleMin, yScaleMax});
    console.log(yScale(6270));
    
    let svg = d3.select("#plotly-div")
      .append("svg").attr("width", "100%").attr("height", svgHeight);

    svg.selectAll("rect")
      .data(d)
      .enter()
      .append("rect")
      .attr("width", 4)
      .attr("height", (o: OhlcRecord) => {
        return Math.abs(yScale(o.open) - yScale(o.close));
      })
      .attr("fill", "red")
      .attr("x", (x: OhlcRecord, i) => {

        return i * 5;
      })
      .attr("y", (y: OhlcRecord, i) => {
        return svgHeight - yScale(y.open);
      });

    // d3.json("", function (data) {
    //   console.log(data[0]);
    // });


    //svg.append("rect").attr("width", 100).attr("height", 50).attr("fill", "red").attr("x", 50).attr("y", 100);
  }
}
