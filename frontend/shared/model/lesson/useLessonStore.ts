import { storeCreate } from "@/shared/lib/storeCreate"
import { TLesson } from "./TLesson"

export const useLessonStore = storeCreate<TLesson[], "lessons">("lessons", [])
