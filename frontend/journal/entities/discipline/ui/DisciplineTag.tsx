import {
	DisciplineType,
	DisciplineTypeShort,
	DISCIPLINE_TYPE_SHORT_MAP,
} from "@/shared/model/discipline"
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
import type { SvgIconProps } from "@mui/material/SvgIcon"
import { memo } from "react"
import BiotechIcon from "@mui/icons-material/Biotech"
import AutoStoriesIcon from "@mui/icons-material/AutoStories"
import DescriptionIcon from "@mui/icons-material/Description"
import RecordVoiceOverIcon from "@mui/icons-material/RecordVoiceOver"
import MessageIcon from "@mui/icons-material/Message"
import QuestionMarkIcon from "@mui/icons-material/QuestionMark"
import { FunctionIcon } from "@/shared/ui/function-icon"
import { AtomIcon } from "@/shared/ui/atom-icon"
import { CertificateIcon } from "@/shared/ui/certificate-icon"


interface Props {
	disciplineType: DisciplineType | DisciplineTypeShort
}

const disciplineIcons: Record<
	DisciplineType | DisciplineTypeShort,
	React.FC<SvgIconProps>
> = {
	"Лекция": AutoStoriesIcon,
	"Лек.": AutoStoriesIcon,
	"Упражнение": FunctionIcon,
	"Упр.": FunctionIcon,
	"Лабораторная": BiotechIcon,
	"Лаб.": BiotechIcon,
	"Семинар": MessageIcon,
	"Сем.": MessageIcon,
	"Курсовой проект": DescriptionIcon,
	"Курс.": DescriptionIcon,
	"Колоквиум": RecordVoiceOverIcon,
	"Колок.": RecordVoiceOverIcon,
	"Доп. курсы": CertificateIcon,
	"НИИР": AtomIcon,
	"Другое": QuestionMarkIcon,
}

const selectColor = (type: DisciplineType | DisciplineTypeShort): TagColor => {
	switch (type) {
		case "Лекция":
		case "Лек.":
			return { bg: green[100], text: green[800] }
		case "Упражнение":
		case "Упр.":
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
	const tagColor = selectColor(disciplineType)
	const Icon = disciplineIcons[disciplineType]
	const shortType =
		disciplineType in DISCIPLINE_TYPE_SHORT_MAP
			? DISCIPLINE_TYPE_SHORT_MAP[disciplineType as DisciplineType]
			: disciplineType

	return (
		<Tag
			name={shortType}
			color={tagColor}
			icon={<Icon fontSize="small" sx={{ color: tagColor.text }} />}
		/>
	)
}

export default memo(DisciplineTag)
