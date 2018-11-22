import { Component, OnInit, HostListener } from '@angular/core';
import * as d3 from "d3";
import { HttpClient } from '@angular/common/http';
import { OhlcRawResponse } from './ohlc/ohlc-raw-response';
import { map, debounceTime } from 'rxjs/operators';
import { OhlcDataRecord } from './ohlc/ohlc-data-record';
import { range, Subject } from 'rxjs';
import { ChartCore } from './core/chart-core';

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.scss']
})
export class ChartComponent implements OnInit {

  private chartCore: ChartCore;
  private onResizedObs: Subject<any> = new Subject();

  public svgHeight: number = 600;

  constructor(
    private httpClient: HttpClient
  ) { }

  public selectedOhlc: OhlcDataRecord = null;

  ngOnInit() {
    this.chartCore = new ChartCore("#plotly-div svg", this.svgHeight);

    this.httpClient.get("./assets/ohlc.json").pipe(map((o) => {
      let rRaw = (<OhlcRawResponse>o).result["XXBTZUSD"];
      let ohlcs: OhlcDataRecord[] = rRaw.map((v) => {
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
      });

      return ohlcs.sort((a, b) => {
        return a.time > b.time ? 1 : -1;
      });
    })).subscribe((d) => {
      // Chanhe all time to indexes. FOR DEBUG.
      // d.forEach((v, i) => {
      //   v.time = i;
      // });

      this.chartCore.setData(d);
      // "https://api.kraken.com/0/public/OHLC?pair=XBTUSD"
      this.chartCore.render();
    });

    this.onResizedObs.pipe(debounceTime(100)).subscribe((d) => {
      this.onResizeDeb(d);
    });

    this.chartCore.onOhlcItemSelected.subscribe((d) => {
      this.selectedOhlc = d;
    });
  }

  public get isBid (): boolean {
    if(!this.selectedOhlc)
      return false;
    return this.selectedOhlc.open < this.selectedOhlc.close;
  }

  @HostListener('window:resize', ['$event'])
  public onResize(event) {
    this.onResizedObs.next(event);
  }

  private onResizeDeb(event) {
    this.chartCore.updateSvgWidth();
  }
}
