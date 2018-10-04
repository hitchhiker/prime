import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TradingPanelComponent } from './trading-panel.component';

describe('TradingPanelComponent', () => {
  let component: TradingPanelComponent;
  let fixture: ComponentFixture<TradingPanelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TradingPanelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TradingPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
