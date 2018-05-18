import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExchangesComponent } from './exchanges.component';

describe('ExchangesComponent', () => {
  let component: ExchangesComponent;
  let fixture: ComponentFixture<ExchangesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExchangesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExchangesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
