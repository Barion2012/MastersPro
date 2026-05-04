<?php

namespace App\Exports;

use App\Models\Order;
use Maatwebsite\Excel\Concerns\FromCollection;
use Maatwebsite\Excel\Concerns\WithHeadings;
use Maatwebsite\Excel\Concerns\WithMapping;
use Maatwebsite\Excel\Concerns\WithColumnWidths;
use Maatwebsite\Excel\Concerns\WithStyles;
use PhpOffice\PhpSpreadsheet\Worksheet\Worksheet;

class OrderDetailExport implements FromCollection, WithHeadings, WithMapping, WithColumnWidths, WithStyles
{
    protected $orderId;

    public function __construct($orderId)
    {
        $this->orderId = $orderId;
    }

    public function collection()
    {
        return Order::find($this->orderId);
    }

    public function headings(): array
    {
        return [
            '№',
            'ФИО',
            'Профессия',
            'Уровень',
            'Гражданство'
        ];
    }

    public function map($order): array
    {
        $detail = [];
        $index = 0;
        $citizenship = '';

        foreach ($order->acceptWorks->where('status', '!=', 'refused') as $acceptWork) {
            $index++;
            $citizenship = 'Прочее';

            if ($acceptWork->worker->citizenship == 'citizen')
                $citizenship = 'Гражданин РФ';
            if ($acceptWork->worker->citizenship == 'eaeu')
                $citizenship = 'ЕАЭС';

            $detail[] = [
                'index' => $index,
                'name' => $acceptWork->user->name,
                'profession' => $acceptWork->profession->name,
                'professionLevel' => $acceptWork->professionLevel->desc,
                'citizenship' => $citizenship,
            ];
        }
        return $detail;
    }

    public function columnWidths(): array
    {
        return [
            'A' => 10,
            'B' => 30,
            'C' => 30,
            'D' => 30,
            'E' => 20,
        ];
    }

    public function styles(Worksheet $sheet)
    {
        return [
            1 => ['font' => ['bold' => true]], 
        ];
    }
}
