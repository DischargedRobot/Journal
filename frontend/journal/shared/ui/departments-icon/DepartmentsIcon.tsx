import SvgIcon, { SvgIconProps } from "@mui/material/SvgIcon"

// Чтобы не юзать use client юзаем переменные стилей
const iconSx = {
	width: 64,
	height: 64,
	"& path": {
		fill: "var(--mui-palette-secondary-light)",
		stroke: "var(--mui-palette-contrastingSecondary-dark)",
		strokeWidth: 2,
	},
} as const

const DepartmentsIcon = ({ sx, ...props }: SvgIconProps) => {
	return (
		<SvgIcon
			{...props}
			viewBox="0 0 64 64"
			// Объединяем переменные стилей с переданными стилями
			sx={[iconSx, ...(Array.isArray(sx) ? sx : [sx])]}
		>
			<path d="M53.9727 18.9131L54.1836 19.0195H56.1904V19.79H7.80957V19.0195H9.81641L10.0273 18.9131L32 7.92676L53.9727 18.9131Z" />
			<path d="M35 25V48H30V25H35Z" />
			<path d="M49 25V48H44V25H49Z" />
			<path d="M21 25V48H16V25H21Z" />
			<path d="M53.4202 52.7695L53.4211 54.4834V55.4834H56.1917V56.1914H7.81079V55.4834H10.5813V52.5898L53.4202 52.7695Z" />
		</SvgIcon>
	)
}

export default DepartmentsIcon
