<?php 

namespace App\Services;

use App\Models\File;
use Illuminate\Support\Facades\Storage;

class FileService {
    public function storeFile($modelType, $modelId, $name, $file, $storage) {
        $this->checkOldFileExist($modelType, $modelId, $name);

        $path = strtolower("files/$modelType/$modelId/");
        $fileModel = new File();
        $uidFilename = uniqid().'.'.$file->getClientOriginalExtension();

        Storage::disk($storage)->putFileAs($path, $file, $uidFilename);

        $fileModel->model_type = "App\Models\\".$modelType;
        $fileModel->model_id = $modelId;
        $fileModel->name = $name;
        $fileModel->storage = $storage;
        $fileModel->path = '/'.$path.$uidFilename;
        $fileModel->filename = $file->getClientOriginalName(); 
        $fileModel->size = $file->getSize();

        $fileModel->save();
    }

    public function getFileDownload($fileId) {
        $file = File::find($fileId);

        if (empty($file))
            return null;

        return Storage::disk($file->storage)->download(str_replace('/storage/', '', $file->path), $this->transliterateFilename($file->filename));
    }

    public function deleteFile($modelType, $modelId, $name) {
        $file = File::where([['model_type', "App\\Models\\".$modelType], ['model_id', $modelId], ['name', $name]])->first();

        if (empty($file))
            return;

        Storage::disk($file->storage)->delete(str_replace('/storage/', '', $file->path));
        $file->delete();
    }

    private function transliterateFilename($filename) 
    {
        $filename = mb_strtolower($filename);
        $filename = trim($filename);
        $filename = str_replace(['(', ')'], '', $filename);
        $ext = pathinfo($filename, PATHINFO_EXTENSION);
        $filename = basename($filename,".".$ext);
        $filename = str_replace(' ', '-', $filename);
        $filename = preg_replace('/[^a-zA-Z0-9а-яА-Я0-9\s.-_]/', '', $filename);
        
        $filename = strtr($filename, array(
            'а' => 'a', 'б' => 'b', 'в' => 'v',
            'г' => 'g', 'д' => 'd', 'е' => 'e',
            'ё' => 'e', 'ж' => 'zh', 'з' => 'z',
            'и' => 'i', 'й' => 'y', 'к' => 'k',
            'л' => 'l', 'м' => 'm', 'н' => 'n',
            'о' => 'o', 'п' => 'p', 'р' => 'r',
            'с' => 's', 'т' => 't', 'у' => 'u',
            'ф' => 'f', 'х' => 'h', 'ц' => 'c',
            'ч' => 'ch', 'ш' => 'sh', 'щ' => 'sch',
            'ь' => '\'', 'ы' => 'y', 'ъ' => '\'',
            'э' => 'e', 'ю' => 'yu', 'я' => 'ya'
        ));

        return $filename.'.'.$ext;
    }

    private function checkOldFileExist($modelType, $modelId, $name) {
        if (str_ends_with($name, '[]'))
            return;

        $oldFile = File::where([['model_type', 'App\Models\\'.$modelType], ['model_id', $modelId], ['name', $name]])->first();

        if (empty($oldFile))
            return;

        $this->deleteFile($modelType, $modelId, $name);
    }
}