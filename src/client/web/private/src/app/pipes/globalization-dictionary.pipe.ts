import { PipeTransform, Pipe } from '@angular/core';
import { GlobalizationDictionaryService } from '../services/globalization-dictionary.service';

@Pipe({ name: 'globalizationdictionary' })
export class GlobalizationDictionaryPipe implements PipeTransform {
    value: string;
    constructor(
        private service: GlobalizationDictionaryService,
    ) {
    }
    transform(key: string): string {
        const keyPrefix = 'glb-dict-';

        if (localStorage.getItem(keyPrefix + key) != null) {
            this.value = localStorage.getItem(keyPrefix + key);
        } else {
            this.service.get(key).subscribe(
                o => {
                    this.value = o.body as string;
                    if (this.value != null) {
                        localStorage.setItem(keyPrefix + key, this.value);
                    }
                }
            );
        }
        return (this.value);
    }
}
