import { DisciplineType, DisciplineTypeShort } from "@/shared/model/discipline"
import { Tag, TagColor } from "@/shared/ui/tag"
import {
	green,
	blue,
	purple,
	lightBlue,
	teal,
	orange,
	cyan,
	grey,
} from "@mui/material/colors"
import { memo } from "react"

interface Props {
	disciplineType: DisciplineType | DisciplineTypeShort
}

const selectColor = (type: DisciplineType | DisciplineTypeShort): TagColor => {
	switch (type) {
		case "Лекция":
		case "Лек.":
			return { bg: green[100], text: green[800] }
		case "Практика":
		case "Прак.":
			return { bg: blue[100], text: blue[700] }
		case "Лабораторная":
		case "Лаб.":
			return { bg: purple[100], text: purple[700] }
		case "Колоквиум":
		case "Колок.":
			return { bg: lightBlue[100], text: lightBlue[700] }
		case "Доп. курсы":
			return { bg: purple[100], text: purple[700] }
		case "НИИР":
			return { bg: teal[100], text: teal[700] }
		case "Курсовой проект":
		case "Курс.":
			return { bg: orange[100], text: orange[700] }
		case "Семинар":
		case "Сем.":
			return { bg: cyan[100], text: cyan[700] }
		case "Другое":
			return { bg: grey[200], text: grey[700] }
		default:
			return { bg: grey[200], text: grey[700] }
	}
}

const DisciplineTag = ({ disciplineType }: Props) => {
	return <Tag name={disciplineType} color={selectColor(disciplineType)} />
}

export default memo(DisciplineTag)
