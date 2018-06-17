import { Component, OnInit, Input } from '@angular/core';
import { Extension } from '../../models/extension';

@Component({
  selector: 'app-ext',
  templateUrl: './ext.component.html',
  styleUrls: ['./ext.component.css']
})
export class ExtComponent implements OnInit {

  @Input() extension: Extension;

  constructor() { }

  ngOnInit() {
  }

}
