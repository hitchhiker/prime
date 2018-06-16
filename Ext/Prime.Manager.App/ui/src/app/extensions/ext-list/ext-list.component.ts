import { Component, OnInit } from '@angular/core';
import { Extension } from '../../models/extension';

@Component({
  selector: 'app-ext-list',
  templateUrl: './ext-list.component.html',
  styleUrls: ['./ext-list.component.css']
})
export class ExtListComponent implements OnInit {

  extensions: Extension[] = [
    { id: "123awd", name: "Prime.Finance" },
    { id: "awdjlk1j2h", name: "Prime.IPFS" },
  ];

  constructor() { }

  ngOnInit() {
  }

}
