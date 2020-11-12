import { PipeTransform, Pipe } from '@angular/core';
import { GlobalizationMessagesService } from '../services/globalization-messages.service';

@Pipe({ name: 'globalizationmessages' })
export class GlobalizationMessagesPipe implements PipeTransform {
    value: string;
    constructor(private service: GlobalizationMessagesService) { }
    transform(key: string): string {
        const keyPrefix = 'glb-msg-';

        if (localStorage.getItem(keyPrefix + key) != null) {
            this.value = localStorage.getItem(keyPrefix + key);
        } else {
            if (key.indexOf(',') > 0) {
                const params = key.split(',');
                if (params.length > 2) {
                    this.service
                        .getByParameter2(params[0], params[1], params[2])
                        .subscribe(o => {
                            this.value = o.body as string;
                            if (this.value != null) {
                                localStorage.setItem(keyPrefix + key, this.value);
                            }
                        });
                } else {
                    this.service.getByParameter(params[0], params[1]).subscribe(o => {
                        this.value = o.body as string;
                        if (this.value != null) {
                            localStorage.setItem(keyPrefix + key, this.value);
                        }
                    });
                }
            } else {
                this.service.get(key).subscribe(o => {
                    this.value = o.body as string;
                    localStorage.setItem(keyPrefix + key, this.value);
                });
            }
        }
        return this.value;
    }
}
