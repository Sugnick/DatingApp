import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { AuthServiceService } from '../_services/auth-service.service';
import { error } from 'util';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input()valuesFromHome: any;
  @Output()cancelRegister = new EventEmitter<boolean>();
  model: any = {};
  constructor(private authService: AuthServiceService) { }

  ngOnInit() {
  }

  register() {
    console.log('Registered');
    this.authService.register(this.model).subscribe(() => {
      console.log('registration successful');
    // tslint:disable-next-line: no-shadowed-variable
    }, error => {
      console.log(error);
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
    console.log('Cancelled');
  }

}
