import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  loggedInUser: string;

  constructor(
    public accountService: AccountService,
    private router: Router,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {

    if (localStorage.getItem("user")) {
      this.loggedInUser = JSON.parse(localStorage.getItem("user").valueOf()).username;
      console.log(this.loggedInUser);
    }
  }

  login() {
    this.accountService.login(this.model).subscribe(response => {
      this.router.navigateByUrl('/members');
      window.location.reload();
    },
      error => {
        console.error(error);
        this.toastr.error(error.error);
      }
    )
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}