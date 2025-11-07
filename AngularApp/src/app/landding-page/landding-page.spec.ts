import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LanddingPage } from './landding-page';

describe('LanddingPage', () => {
  let component: LanddingPage;
  let fixture: ComponentFixture<LanddingPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LanddingPage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LanddingPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
