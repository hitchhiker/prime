import { OhlcDataRecord } from "../ohlc/ohlc-data-record";
import * as d3 from "d3";
import { Observable, Subject } from "rxjs";
import { ChartDragger, Point } from "../chart-dragger";
import { OhlcChartItem } from "./ohlc-chart-item";
import { SvgCore } from "./svg-core";
import { RenderingCtx } from "./rendering-ctx";


export class ChartCore {
    private _chartItems: OhlcChartItem[];
    private _chartItemsInView: OhlcChartItem[];

    private _chartDragger: ChartDragger = new ChartDragger();
    private _svgCore: SvgCore = new SvgCore();

    private svg;
    private gMain;
    private gLeftAxis;
    private selectionCrosshair = { horizontal: null, vertical: null };

    private _onOhlcItemSelected: Subject<OhlcDataRecord> = new Subject();
    public onOhlcItemSelected: Observable<OhlcDataRecord> = this._onOhlcItemSelected.asObservable();

    private selectedOhlcRecord: OhlcDataRecord;

    constructor(selector: string) {
        this.initialize(selector);

        this._chartDragger.dragMoved.subscribe((p) => {
            this._svgCore.svgDragMovingHandler(p);
        });
        this._chartDragger.dragStarted.subscribe((p) => {
            this._svgCore.svgDragStartedHandler(p);
        });
        this._chartDragger.dragEnded.subscribe((dp) => {
            this._svgCore.svgDragStartedHandler(dp);
        });
    }

    private _sizing = {
        margin: {
            top: 20, right: 50, bottom: 10, left: 0
        },
        chartOffset: {
            max: 100,
            min: 0 // Initialized during runtime.
        },
        bars: {
            width: 10,
            gap: 3,
            maxWidth: 50,
            minWidth: 3
        },
        height: 450,
        width: 0
    };

    public updateSvgWidth() {
        this._svgCore.setSvgWidth();
        this.render();
    }

    public moveLeft() {
        this.chartOffsetX += this._sizing.bars.width + this._sizing.bars.gap;
    }
    public moveRight() {
        this.chartOffsetX -= this._sizing.bars.width + this._sizing.bars.gap;
    }

    public set chartOffsetX(v: number) {
        this._svgCore.chartOffsetX = v;
        this.render();
    }
    public get chartOffsetX(): number {
        return this._svgCore.chartOffsetX;
    }

    public set barWidth(v: number) {
        if (v > this._sizing.bars.maxWidth || v < this._sizing.bars.minWidth)
            return;

        let displacement = this.calcZoomOffsetDisplacement(v);

        this._sizing.bars.width = v;
        this._svgCore.chartOffsetXInitialDiff = null;

        this.chartOffsetX -= displacement;
        this.render();
    }
    public get barWidth(): number {
        return this._sizing.bars.width;
    }

    public initialize(selector: string) {
        this._svgCore.initialize(d3.select(selector), this._sizing);
        this._svgCore.registerSvgHandlers();

        this._svgCore.onSvgMouseMove.subscribe((p) => {
            this._chartDragger.mouseMove(p);
        });
        this._svgCore.onSvgMouseWheel.subscribe((v) => {
            this.barWidth += v / 100;
        });
        this._svgCore.onSvgMouseDown.subscribe((o) => {
            this._chartDragger.dragStart();
        });
        this._svgCore.onSvgMouseUp.subscribe((o) => {
            this._chartDragger.dragEnd();
        });

        this._svgCore.createControls();
    }

    public setData(data: OhlcDataRecord[]) {
        this._chartItems = data.map((x, i) => {
            let r: OhlcChartItem = { ohlc: x, posX: this.getBarPosX(i) };
            return r;
        });

        this._svgCore.chartOffsetX = -this._chartItems[this._chartItems.length - 50].posX;
        
        this._onOhlcItemSelected.next(this._chartItems[this._chartItems.length - 1].ohlc);
        //this.viewPort.x1 = this.getXbyIndex(data.length - 1);
    }

    private recalcItemsPosX(data: OhlcChartItem[]): OhlcChartItem[] {
        data.forEach((v, i) => {
            v.posX = this.getBarPosX(i);
        });

        let lastItem = data[data.length - 1];
        this._sizing.chartOffset.min = -lastItem.posX + this._sizing.width - 100;

        return data;
    }

    public getBarPosX(i: number): number {
        return i * (this._sizing.bars.width + this._sizing.bars.gap)
    }

    public getInView(data: OhlcChartItem[]): OhlcChartItem[] {
        let inView = data.filter((record, i) => {
            let startX = -(this._svgCore.chartOffsetX);
            let endX = startX + this._sizing.width - (this._sizing.margin.right + this._sizing.bars.width) - this._sizing.margin.left;
            let currX = record.posX;

            let r = currX >= startX && currX <= endX;

            return r;
        });

        return inView;
    }

    // Calculates center of chart view (SVG view) in chart data coordinates (taking into account chart offset X).
    private calcZoomOffsetDisplacement(newWidth: number): number {
        let offsetAndWidth = -this._svgCore.chartOffsetX + this._sizing.width;
        let itemsInViewOld = offsetAndWidth / (this._sizing.bars.width + this._sizing.bars.gap);
        let itemsInViewNew = offsetAndWidth / (newWidth + this._sizing.bars.gap);
        let rel = itemsInViewOld / itemsInViewNew;
        let partDiff = (rel * offsetAndWidth - offsetAndWidth) * ((offsetAndWidth - (this._sizing.width - this._chartDragger.mousePos.x)) / offsetAndWidth);
        return partDiff;
    }

    private yScaleRawReversed;

    public render() {
        let self = this;
        let svg = this.svg;
        let allData = this.recalcItemsPosX(this._chartItems);
        let sizing = this._sizing;
        let gMain = this.gMain;
        let gLeftAxis = this.gLeftAxis;

        this._chartItemsInView = this.getInView(allData);
        let dataInView = this._chartItemsInView;

        let ctx: RenderingCtx = {

        };

        this._svgCore.render(dataInView);
    }
}
