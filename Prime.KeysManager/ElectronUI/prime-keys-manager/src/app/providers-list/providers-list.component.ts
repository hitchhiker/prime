import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-providers-list',
  templateUrl: './providers-list.component.html',
  styleUrls: ['./providers-list.component.css']
})
export class ProvidersListComponent implements OnInit {
  @Input() providers: Array<any>;

  constructor() { }

  ngOnInit() {
  }

}
