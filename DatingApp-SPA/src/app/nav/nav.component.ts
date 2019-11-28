import { Component, OnInit } from '@angular/core';
import { AuthServiceService } from '../_services/auth-service.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}; // Hold the User Name and Paassword
  constructor(private authService: AuthServiceService) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(
      next => {
        console.log('Login Successful');
      }, error => {
        console.log('Failed to login');
      });
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !!token;
  }

  logout() {
    localStorage.removeItem('token');
  }
}
