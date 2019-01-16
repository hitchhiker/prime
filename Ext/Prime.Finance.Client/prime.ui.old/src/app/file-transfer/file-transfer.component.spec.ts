import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FileTransferComponent } from './file-transfer.component';

describe('FileTransferComponent', () => {
  let component: FileTransferComponent;
  let fixture: ComponentFixture<FileTransferComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FileTransferComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FileTransferComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
