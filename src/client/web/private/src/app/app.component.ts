import { Component, OnInit } from '@angular/core';
import { MainService } from './services/main.service';
import { GlobalizationDictionaryPipe } from './pipes/globalization-dictionary.pipe';
import { Title } from '@angular/platform-browser';
import { KeyValue } from './value-objects/key-value';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
    loading: boolean;
    cachedValues: KeyValue[] = [];
    constructor(
        private titleService: Title,
        private serviceMain: MainService,
        private globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    ) { }

    ngOnInit(): void  {
        this.loading = true;
        const title = this.globalizationDictionaryPipe.transform('ApplicationName');
        this.titleService.setTitle(title);
        this.serviceMain.globalizationKeys().subscribe(
            res => {
                if (res.status === 200) {
                    let keys: KeyValue[];
                    keys = res.body as KeyValue[];

                    keys.forEach(keyValuePair => {
                        if (localStorage.getItem(keyValuePair.key) == null) {
                            localStorage.setItem(keyValuePair.key, keyValuePair.value);
                            this.cachedValues.push(keyValuePair);
                        }
                    });

                    this.loading = false;

                } else {
                }
            },
            err => {
                this.loading = false;
            }
        );
    }
}
