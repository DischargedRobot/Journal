import SvgIcon, { SvgIconProps } from "@mui/material/SvgIcon"

const boxPath =
	"M15.4606 13.8022C15.4606 14.2419 15.2859 14.6636 14.975 14.9745C14.6641 15.2854 14.2424 15.4601 13.8027 15.4601H2.19745C1.75774 15.4601 1.33605 15.2854 1.02514 14.9745C0.714221 14.6636 0.539551 14.2419 0.539551 13.8022V2.19696C0.539551 1.75726 0.714221 1.33556 1.02514 1.02465C1.33605 0.713733 1.75774 0.539063 2.19745 0.539062H13.4719C14.572 0.539062 15.4656 1.42761 15.4719 2.52774L15.5 7.49951L15.4606 13.8022Z"

const iconSx = {
	width: 16,
	height: 16,
	"& path": {
		fill: "none",
		stroke: "var(--mui-palette-contrastingSecondary-main)",
		strokeLinecap: "round",
		strokeLinejoin: "round",
	},
} as const

const CheckboxRectUncheckedIcon = ({ sx, ...props }: SvgIconProps) => (
	<SvgIcon
		{...props}
		viewBox="0 0 16 16"
		sx={[iconSx, ...(Array.isArray(sx) ? sx : [sx])]}
	>
		<path d={boxPath} />
	</SvgIcon>
)

export default CheckboxRectUncheckedIcon
