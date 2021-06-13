import { Component, OnInit } from '@angular/core';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  loggedIn!: boolean;
  loggedInUser: string;

  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
    this.getCurrentUser();

    if (localStorage.getItem("user")) {
      this.loggedInUser = JSON.parse(localStorage.getItem("user").valueOf()).username;
      console.log(this.loggedInUser);
    }
  }

  login() {
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
      this.loggedIn = true;
    },
      error => {
        console.error(error);
      }
    )
  }

  logout() {
    this.accountService.logout();
    this.loggedIn = false;
  }

  getCurrentUser() {
    this.accountService.currentUser$.subscribe(user => {
      this.loggedIn = !!user;
    },
      error => {
        console.log(error);
      })
  }
}
