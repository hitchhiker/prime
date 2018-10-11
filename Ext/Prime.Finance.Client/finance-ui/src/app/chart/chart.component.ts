import { Component, OnInit } from '@angular/core';
import * as d3 from "d3";

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.scss']
})
export class ChartComponent implements OnInit {

  constructor() { }

  ngOnInit() {
    this.draw();
  }

  private draw() {
    //let svg = d3.select("#plotly-div svg");
    //svg.append("rect").attr("width", 100).attr("height", 50).attr("fill", "red").attr("x", 50).attr("y", 100);
  }
}
