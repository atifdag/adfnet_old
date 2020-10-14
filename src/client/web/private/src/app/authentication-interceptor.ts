import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IdentityService } from './services/identity.service';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';

@Injectable()
export class AuthenticationInterceptor implements HttpInterceptor {

    token: string;
    headers: HttpHeaders;

    constructor(
        private identityService: IdentityService,
        private router: Router
    ) {
    }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (this.identityService.isLogged()) {
            this.token = this.identityService.get();
        } else {
            this.token = '';
        }

        this.headers = new HttpHeaders({
            'Content-Type': 'application/json',
            Authorization: this.token
        });



        const updatedRequest = request.clone({ headers: this.headers });
        return next.handle(updatedRequest).pipe(
            tap(
                event => {
                    // console.log('event :', event);
                    if (event instanceof HttpResponse) {
                        if (event.status === 401) {
                            this.router.navigate(['/User/MyProfile']);
                            // console.log('event 401 :', event);
                        } else {
                            // console.log('api call success :', event);
                        }
                    }
                },
                error => {
                    if (error.status === 401) {
                        console.log('Yetkisiz:', error.status);
                        this.router.navigate(['/User/MyProfile']);
                        // this.messageService.add(
                            // { severity: 'error', summary: 'Yetkisiz Giriş!', detail: 'Bu işlem için yetkiniz yok!' });
                    } else {
                        console.log('api call error :', error);
                      //  this.router.navigate(['/User/MyProfile']);
                    }
                }
            )
        );
    }
}
