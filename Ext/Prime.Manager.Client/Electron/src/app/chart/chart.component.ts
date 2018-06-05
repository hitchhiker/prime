import { Component, OnInit } from '@angular/core';
import { ChartService } from '../services/chart.service';

declare var drawChart: () => void;

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css']
})
export class ChartComponent implements OnInit {
  constructor(
    private chartService: ChartService
  ) { 
    chartService.load.subscribe(x => this.loadedHandler());
  }

  loadedHandler() {
    drawChart();
  }

  ngOnInit() {
    
  }

}
