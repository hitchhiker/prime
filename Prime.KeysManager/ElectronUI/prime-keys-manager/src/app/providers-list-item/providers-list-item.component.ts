import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-providers-list-item',
  templateUrl: './providers-list-item.component.html',
  styleUrls: ['./providers-list-item.component.css']
})
export class ProvidersListItemComponent implements OnInit {
  @Input() provider: any;

  constructor() { }

  ngOnInit() {
  }

}
