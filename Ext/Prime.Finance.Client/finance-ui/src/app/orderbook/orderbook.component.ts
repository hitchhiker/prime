import { Component, OnInit, HostListener, ViewChild, TemplateRef } from '@angular/core';
import { OrderbookOrder } from 'src/models/view/orderbook-order';
import { OrderbookService } from '../services/orderbook/orderbook.service';
import { ResponseType } from '../services/last-trades/bitfinex/response-type';
import { OrderSide } from '../models/trading/order-side';

@Component({
  selector: 'app-orderbook',
  templateUrl: './orderbook.component.html',
  styleUrls: ['./orderbook.component.scss']
})
export class OrderbookComponent implements OnInit {

  orderbookBids: OrderbookOrder[];
  orderbookAsks: OrderbookOrder[];

  private readonly maxNumberOfOrderbookOrders: number = 30;
  private readonly ordersResetCount: number = this.maxNumberOfOrderbookOrders * 1.5;


  constructor(
    private orderBookService: OrderbookService
  ) { }

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

    this.orderBookService.connect();
    this.orderBookService.orderbook.subscribe((msg) => {
      if (msg) {
        if (msg.responseType === ResponseType.Snapshot) {
          this.orderbookBids = (<OrderbookOrder[]>msg.payload).filter((o) => o.Side === OrderSide.Buy);
          this.orderbookAsks = (<OrderbookOrder[]>msg.payload).filter((o) => o.Side === OrderSide.Sell);
        }
        else if(msg.responseType === ResponseType.TransactionUpdated) {
          let record = <OrderbookOrder>msg.payload;
          if(record.Price === 0) {
            this.orderbookAsks = this.orderbookAsks.filter((r) => {
              return r.Id !== record.Id;
            });
            this.orderbookBids = this.orderbookBids.filter((r) => {
              return r.Id !== record.Id;
            });

          } else {
            if(record.Side === OrderSide.Buy) {
              this.orderbookBids.unshift(record);
            } else {
              this.orderbookAsks.unshift(record);
            }
          }
        }

        console.log(this.orderbookBids.length);
        

        if(this.orderbookBids.length > this.ordersResetCount) {
          this.orderbookBids.length = this.maxNumberOfOrderbookOrders;
        }
        if(this.orderbookAsks.length > this.ordersResetCount) {
          this.orderbookAsks.length = this.maxNumberOfOrderbookOrders;
        }
  
        this.orderbookBids = this.orderbookBids.sort((a, b) => {
          return a.Price > b.Price ? -1: 1;
        });
        this.orderbookAsks = this.orderbookAsks.sort((a, b) => {
          return a.Price > b.Price ? 1: -1;
        });
      }
    });
  }
}
