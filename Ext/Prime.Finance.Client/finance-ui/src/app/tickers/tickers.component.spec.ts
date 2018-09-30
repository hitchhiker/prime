import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TickersComponent } from './tickers.component';

describe('TickersComponent', () => {
  let component: TickersComponent;
  let fixture: ComponentFixture<TickersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TickersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TickersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
