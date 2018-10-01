import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LastTradesComponent } from './last-trades.component';

describe('LastTradesComponent', () => {
  let component: LastTradesComponent;
  let fixture: ComponentFixture<LastTradesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LastTradesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LastTradesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
