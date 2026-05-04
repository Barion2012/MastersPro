<?php

namespace App\Http\Controllers;

use App\Models\Profession;
use App\Models\ProfessionLevel;
use Illuminate\Database\QueryException;
use Illuminate\Http\Request;

class ProfessionControler extends Controller
{
    public function getProfessionListDetailed(Request $request) {
        $professions = Profession::paginate(10);
        $currentPage = $professions->currentPage();
        $perPage = $professions->perPage();

        return [
            'data' => $professions->getCollection()->transform(function ($profession, $key) use ($currentPage, $perPage) {
                return [
                    'id' => $profession->id,
                    'name' => $profession->name,
                    'desc' => $profession->desc,
                    'levels' => $profession->level_list
                ];
            }),
            'total' => $professions->total(),
            'current_page' => $currentPage,
            'per_page' => $perPage
        ];
    }

    public function getProfessionListSelect(Request $request) {
        $professions = Profession::all();

        $response = [
            'professions' => [],
            'levels' => [],
            'desc' => []
        ];

        foreach ($professions as $profession) {
            $response['professions'][] = [
                'value' => $profession->id,
                'text' => $profession->name,
            ];

            $response['desc'][$profession->id] = $profession->desc;

            foreach ($profession->level_list as $level) {
                $response['levels'][$profession->id][] = [
                    'value' => $level['id'],
                    'text' => $level['desc'],
                    'price' => $level['price'],
                ];
            }
        }

        return $response;
    }

    public function saveProfession(Request $request) {
        $request->validate([
            'name' => ['required', 'string'],
            'desc' => ['string'],
            'id' => ['exists:professions,id'],
        ], [
            'name.required' => 'Название профессии обязательно для заполнения',
            'name.string' => 'Название профессии должно быть текстом',
            'desc.string' => 'Описание профессии должно быть текстом',
            'id.exists' => 'Профессия не найдена',
        ]);

        if (!empty($request->id))
            $profession = Profession::find($request->id);
        else 
            $profession = new Profession();

        $profession->name = $request->name;
        $profession->desc = $request->desc;
        $profession->save();
    }

    public function saveProfessionLevel(Request $request) {
        $request->validate([
            'desc' => ['required', 'string'],
            'profession_id' => ['required', 'exists:professions,id'],
            'price' => ['required', 'numeric', 'min:0', 'max:999999'],
            'id' => ['exists:profession_levels,id'],
        ], [
            'desc.required' => 'Название уровня профессии обязательно для заполнения',
            'desc.string' => 'Название уровня профессии должно быть текстом',
            'profession_id.required' => 'ID профессии обязателен для заполнения',
            'profession_id.exists' => 'Профессия не найдена',
            'price.required' => 'Цена обязательна для заполнения',
            'price.numeric' => 'Цена должна быть числом',
            'price.min' => 'Цена должна быть больше 0',
            'price.max' => 'Цена должна быть меньше 999999',
            'id.exists' => 'Уровень профессии не найден',
        ]);

        if (!empty($request->id))
            $professionLevel = ProfessionLevel::find($request->id);
        else
            $professionLevel = new ProfessionLevel();

        $levelIndex = ProfessionLevel::where('profession_id', $request->profession_id)->max('level') + 1;
        
        $professionLevel->level = $levelIndex;
        $professionLevel->desc = $request->desc;
        $professionLevel->profession_id = $request->profession_id;
        $professionLevel->price = $request->price;
        $professionLevel->save();
    }

    public function deleteProfessionLevel(Request $request) {
        $request->validate([
            'id' => ['required', 'exists:profession_levels,id'],
        ], [
            'id.required' => 'ID уровня профессии обязателен для заполнения',
            'id.exists' => 'Уровень профессии не найден',
        ]);

        $professionLevel = ProfessionLevel::find($request->id);
        $level = $professionLevel->level;

        try {
            $professionLevel->delete();
        }
        catch (QueryException $e) {
            $this->createValidationErrorResponse('Нельзя удалить, так как существуют связанные записи');
        }

        ProfessionLevel::where('profession_id', $professionLevel->profession_id)
            ->where('level', '>', $professionLevel->level)
            ->decrement('level');

        $professionLevel->delete();
    }

    public function deleteProfession(Request $request) {
        $request->validate([
            'id' => ['required', 'exists:professions,id'],
        ], [
            'id.required' => 'ID профессии обязателен для заполнения',
            'id.exists' => 'Профессия не найдена',
        ]);

        $profession = Profession::find($request->id);
        $profession->delete();
    }
}
