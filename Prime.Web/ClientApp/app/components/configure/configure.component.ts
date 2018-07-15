import { Component, OnInit } from "@angular/core";
import * as EventSource from 'eventsource';

@Component({
    selector: 'configure',
    templateUrl: './configure.component.html'
})
export class ConfigureComponent implements OnInit {
    serverUtcTime: string = "fetching...";

    ngOnInit(): void {
        var source = new EventSource('api/Configure/ServerTime');

        source.onmessage = (event: any) => {
            this.sseOnMessage(event);
        };

        source.onopen = function(event: any) {
            console.log('onopen');
        };

        source.onerror = function(event: any) {
            console.log('onerror');
        }
    }

    private sseOnMessage(event: any) {
        this.serverUtcTime = event.data;
    }
}
