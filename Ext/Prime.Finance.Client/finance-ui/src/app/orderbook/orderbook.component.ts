import { Component, OnInit, HostListener, ViewChild, TemplateRef } from '@angular/core';
import { OrderbookOrder } from 'src/models/view/orderbook-order';
import { OrderType } from 'src/models/order-type';

@Component({
  selector: 'app-orderbook',
  templateUrl: './orderbook.component.html',
  styleUrls: ['./orderbook.component.scss']
})
export class OrderbookComponent implements OnInit {

  orderbookBids: OrderbookOrder[] = [
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6700 }
  ];

  orderbookAsks: OrderbookOrder[] = [
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 },
    { Sum: 12, Amount: 1.3, Value: 12412, BidAsk: 6800 }
  ];

  constructor() { }

  tableHeadWitdhBid: number;
  tableHeadWitdhAsk: number;
  @ViewChild('bidContainer') private bidContainer;
  @ViewChild('askContainer') private askContainer;

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    this.checkWidth();
  }

  private checkWidth() {
    this.tableHeadWitdhBid = this.bidContainer.nativeElement.offsetWidth;
    this.tableHeadWitdhAsk = this.askContainer.nativeElement.offsetWidth;
  }

  ngOnInit() {
    this.checkWidth();
  }
}
