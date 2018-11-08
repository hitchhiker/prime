import { Component, OnInit } from '@angular/core';
import * as d3 from "d3";
import { HttpClient } from '@angular/common/http';
import { OhlcRawResponse } from './ohlc/ohlc-raw-response';
import { map } from 'rxjs/operators';
import { OhlcRecord } from './ohlc/ohlc-record';
import { range } from 'rxjs';
import { ChartCore, Viewport } from './chart-core';

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.scss']
})
export class ChartComponent implements OnInit {

  private chartCore: ChartCore;

  constructor(
    private httpClient: HttpClient
  ) { }

  ngOnInit() {
    this.chartCore = new ChartCore("#plotly-div");

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
      // Chanhe all time to indexes. FOR DEBUG.
      d.forEach((v, i) => {
        v.time = i;
      });

      this.chartCore.setData(d);
      this.draw(d);
    });
  }

  public moveLeft() {
    this.chartCore.viewport = new Viewport(this.chartCore.viewport.x1 - 50);
    this.chartCore.render();
  }

  public moveRight() {
    this.chartCore.viewport = new Viewport(this.chartCore.viewport.x1 + 50);
    this.chartCore.render();
  }

  public zoomIn() {

  }

  public zoomOut() {

  }

  // data should be sorted by time.
  private draw(data: OhlcRecord[]) {
    // "https://api.kraken.com/0/public/OHLC?pair=XBTUSD"

    this.chartCore.render();
  }
}
