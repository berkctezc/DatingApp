import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr/toastr/toastr.module';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
      progressBar: true,
      progressAnimation: 'decreasing',
      closeButton: true,
      maxOpened: 3,
      tapToDismiss: true,
      timeOut: 2500
    })
  ],
  exports: [
    BsDropdownModule,
    ToastrModule
  ]
})
export class SharedModule { }